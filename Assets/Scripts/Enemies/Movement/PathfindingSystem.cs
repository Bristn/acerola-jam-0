using System.Collections.Generic;
using Enemies;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial struct PathfindingSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        new TestJob() { ecbParallel = ecb.AsParallelWriter() }.ScheduleParallel();
    }

    [BurstCompile]
    private partial struct TestJob : IJobEntity
    {
        internal EntityCommandBuffer.ParallelWriter ecbParallel;

        // Parameters = Query
        public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref PathfindingParametersData parameters, ref DynamicBuffer<PathPosition> buffer, ref PathFollowData followData)
        {
            int2 startPosition = parameters.Start;
            int2 endPosition = parameters.End;
            int2 gridSize = new(20, 20);

            // Setup nodes
            // TODO: Get nodes from actual tilemap (Convert gameobject data to the required struct)
            NativeArray<PathNode> nodes = new(gridSize.x * gridSize.y, Allocator.Temp);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode node = new()
                    {
                        X = x,
                        Y = y,
                        Index = this.GetCellIndex(x, y, gridSize.x),
                        GCost = int.MaxValue,
                        HCost = this.GetHeuristicCost(new(x, y), endPosition),
                        IsWalkable = true,
                        PreviousNodeIndex = -1
                    };

                    node.UpdateFCost();
                    nodes[node.Index] = node;
                }
            }

            NativeArray<int2> cellNeighborOffset = new(8, Allocator.Temp);
            cellNeighborOffset[0] = new(-1, 0); // Left
            cellNeighborOffset[1] = new(1, 0);  // Right
            cellNeighborOffset[2] = new(-1, 1); // Top Left
            cellNeighborOffset[3] = new(0, 1);  // Top
            cellNeighborOffset[4] = new(1, 1);  // Top Right
            cellNeighborOffset[5] = new(-1, 1); // Bottom Right
            cellNeighborOffset[6] = new(0, 1);  // Bottom
            cellNeighborOffset[7] = new(-1, -1);// Bottom Left

            PathNode startNode = nodes[this.GetCellIndex(startPosition.x, startPosition.y, gridSize.x)];
            startNode.GCost = 0;
            startNode.UpdateFCost();
            nodes[startNode.Index] = startNode;

            NativeList<int> open = new(Allocator.Temp);
            NativeList<int> closed = new(Allocator.Temp);
            int endIndex = this.GetCellIndex(endPosition.x, endPosition.y, gridSize.x);

            open.Add(startNode.Index);
            while (open.Length != 0)
            {
                int currentIndex = this.GetLowesetFCostNodeIndex(open, nodes);
                PathNode currentNode = nodes[currentIndex];

                // Check if reached end
                if (currentIndex == endIndex)
                {
                    break;
                }

                // Remove entry from open node
                for (int i = 0; i < open.Length; i++)
                {
                    if (currentIndex == open[i])
                    {
                        open.RemoveAtSwapBack(i);
                        break;
                    }
                }

                // Add to closed list
                closed.Add(currentIndex);

                for (int i = 0; i < cellNeighborOffset.Length; i++)
                {
                    int2 offset = cellNeighborOffset[i];
                    int2 neighbor = new(currentNode.X + offset.x, currentNode.Y + offset.y);

                    // Ignore cells outside of grid
                    if (!this.IsValidCellIndex(neighbor, gridSize))
                    {
                        continue;
                    }

                    int neighborIndex = this.GetCellIndex(neighbor.x, neighbor.y, gridSize.x);

                    // Ignore already searched nodes
                    if (closed.Contains(neighborIndex))
                    {
                        continue;
                    }

                    PathNode neighborNode = nodes[neighborIndex];
                    if (!neighborNode.IsWalkable)
                    {
                        continue;
                    }

                    // Update the cost of the neighbor
                    int2 currentPosition = new(currentNode.X, currentNode.Y);
                    int newGCost = currentNode.GCost + this.GetHeuristicCost(currentPosition, neighbor);
                    if (newGCost < neighborNode.GCost)
                    {
                        neighborNode.PreviousNodeIndex = currentNode.Index;
                        neighborNode.GCost = newGCost;
                        neighborNode.UpdateFCost();
                        nodes[neighborNode.Index] = neighborNode;

                        // If not already present, add neghbor to searchable nodes
                        if (!open.Contains(neighborNode.Index))
                        {
                            open.Add(neighborNode.Index);
                        }
                    }
                }
            }

            // 
            PathNode endNode = nodes[endIndex];
            buffer.Clear();
            this.BuildPathFromNodes(nodes, endNode, buffer);

            followData.Index = buffer.Length - 1;

            nodes.Dispose();
            open.Dispose();
            closed.Dispose();
            cellNeighborOffset.Dispose();

            // EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
            // ecb.RemoveComponent<PathfindingParametersData>(entity);

            ecbParallel.RemoveComponent<PathfindingParametersData>(sortKey, entity);
        }

        private NativeList<int2> BuildPathFromNodes(NativeArray<PathNode> nodes, PathNode endNode)
        {
            if (endNode.PreviousNodeIndex == -1)
            {
                return new(Allocator.Temp);
            }

            NativeList<int2> path = new(Allocator.Temp)
            {
                new(endNode.X, endNode.Y)
            };

            PathNode currentNode = endNode;
            while (currentNode.PreviousNodeIndex != -1)
            {
                currentNode = nodes[currentNode.PreviousNodeIndex];
                path.Add(new(currentNode.X, currentNode.Y));
            }

            return path;
        }

        private void BuildPathFromNodes(NativeArray<PathNode> nodes, PathNode endNode, DynamicBuffer<PathPosition> buffer)
        {
            if (endNode.PreviousNodeIndex == -1)
            {
                return;
            }

            buffer.Add(new() { Position = new(endNode.X, endNode.Y) });

            PathNode currentNode = endNode;
            while (currentNode.PreviousNodeIndex != -1)
            {
                currentNode = nodes[currentNode.PreviousNodeIndex];
                buffer.Add(new() { Position = new(currentNode.X, currentNode.Y) });
            }
        }

        private int GetCellIndex(int x, int y, int width)
        {
            return x + y * width;
        }

        private bool IsValidCellIndex(int2 index, int2 gridSize)
        {
            if (index.x < 0 || index.y < 0)
            {
                return false;
            }

            if (index.x >= gridSize.x || index.y >= gridSize.y)
            {
                return false;
            }

            return true;
        }

        private int GetHeuristicCost(int2 from, int2 to)
        {
            int xDistance = math.abs(from.x - to.x);
            int yDistance = math.abs(from.y - to.y);
            int remaining = math.abs(xDistance - yDistance);
            return math.min(xDistance, yDistance) + remaining;
        }

        private int GetLowesetFCostNodeIndex(NativeList<int> open, NativeArray<PathNode> nodes)
        {
            PathNode lowestCostNode = nodes[open[0]];
            for (int i = 1; i < open.Length; i++)
            {
                PathNode currentNode = nodes[open[i]];
                if (currentNode.FCost < lowestCostNode.FCost)
                {
                    lowestCostNode = currentNode;
                }
            }

            return lowestCostNode.Index;
        }

        public struct PathNode
        {
            public int X;
            public int Y;
            public int Index;
            public bool IsWalkable;
            public int PreviousNodeIndex;

            public int GCost; // Move cust from start node to this node
            public int HCost; // Estimated cost from here to finish
            public int FCost; // Sum of G + H

            public void UpdateFCost()
            {
                this.FCost = this.GCost + this.HCost;
            }
        }
    }
}