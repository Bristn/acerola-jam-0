using System.Collections.Generic;
using Pathfinding.Algorithm;
using Pathfinding.Followers;
using Pathfinding.Positions;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Pathfinding
{
    public partial struct PathfindingSystem : ISystem
    {
        private NativeArray<int2> evenNeighborOffsets;
        private NativeArray<int2> oddNeighborOffsets;

        public void OnCreate(ref SystemState state)
        {
            this.evenNeighborOffsets = PathHelpers.EvenNeighborOffsets;
            this.oddNeighborOffsets = PathHelpers.OddNeighborOffsets;
            state.RequireForUpdate<TilemapData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // Check if tilemap is initialized
            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();
            if (!tilemapData.IsUpdated)
            {
                return;
            }

            // Convert buffer to list
            DynamicBuffer<TilemapNodesData> tileBuffer = SystemAPI.GetSingletonBuffer<TilemapNodesData>();
            NativeArray<PathNode> baseNodes = new(tileBuffer.Length, Allocator.TempJob);
            for (int i = 0; i < tileBuffer.Length; i++)
            {
                baseNodes[i] = tileBuffer[i].Node;
            }

            NativeArray<JobHandle> handles = new(1, Allocator.Temp);
            JobHandle handle = new FindPathJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                EvenNeighborOffsets = this.evenNeighborOffsets,
                OddNeighborOffsets = this.oddNeighborOffsets,
                GridSize = tilemapData.GridSize,
                AllNodes = baseNodes,
            }.ScheduleParallel(new JobHandle());
            handles[0] = handle;

            JobHandle.CompleteAll(handles);

            handles.Dispose();
            baseNodes.Dispose();
        }

        [BurstCompile]
        private partial struct FindPathJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public NativeArray<int2> EvenNeighborOffsets; // Disable savety as access is readonly
            [NativeDisableParallelForRestriction] public NativeArray<int2> OddNeighborOffsets; // Disable savety as access is readonly
            public NativeArray<PathNode> AllNodes;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public int2 GridSize;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref PathfindingRequestPathData parameters, ref DynamicBuffer<PathPosition> buffer, ref PathFollowerData followData)
            {
                NativeArray<PathNode> nodes = new(this.AllNodes.Length, Allocator.Temp);
                nodes.CopyFrom(this.AllNodes);

                int2 startPosition = parameters.StartCell;
                int2 endPosition = parameters.EndCell;

                // Setup nodes
                for (int i = 0; i < nodes.Length; i++)
                {
                    PathNode node = nodes[i];
                    node.HCost = PathHelpers.GetHeuristicCost(new(node.X, node.Y), endPosition);
                    node.PreviousNodeIndex = -1;

                    nodes[i] = node;
                }

                PathNode startNode = nodes[PathHelpers.GetCellIndex(startPosition.x, startPosition.y, this.GridSize.x)];
                startNode.GCost = 0;
                startNode.UpdateFCost();
                nodes[startNode.Index] = startNode;

                NativeList<int> open = new(Allocator.Temp);
                NativeList<int> closed = new(Allocator.Temp);
                int endIndex = PathHelpers.GetCellIndex(endPosition.x, endPosition.y, this.GridSize.x);

                open.Add(startNode.Index);
                while (open.Length != 0)
                {
                    int currentIndex = PathHelpers.GetLowesetFCostNodeIndex(open, nodes);
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
                    int neighborCount = this.OddNeighborOffsets.Length;
                    for (int i = 0; i < neighborCount; i++)
                    {
                        int2 offset = currentNode.Y % 2 == 0 ? this.EvenNeighborOffsets[i] : this.OddNeighborOffsets[i];
                        int2 neighbor = new(currentNode.X + offset.x, currentNode.Y + offset.y);

                        // Ignore cells outside of grid
                        if (!PathHelpers.IsValidCellIndex(neighbor, this.GridSize))
                        {
                            continue;
                        }

                        int neighborIndex = PathHelpers.GetCellIndex(neighbor.x, neighbor.y, this.GridSize.x);

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
                        int newGCost = currentNode.GCost + PathHelpers.GetHeuristicCost(currentPosition, neighbor);
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

                followData.CurrentCellIndex = buffer.Length - 1;

                open.Dispose();
                closed.Dispose();
                nodes.Dispose();

                CommandBuffer.RemoveComponent<PathfindingRequestPathData>(sortKey, entity);
            }

            private void BuildPathFromNodes(NativeArray<PathNode> nodes, PathNode endNode, DynamicBuffer<PathPosition> buffer)
            {
                if (endNode.PreviousNodeIndex == -1)
                {
                    return;
                }

                buffer.Add(new() { GridPosition = new(endNode.X, endNode.Y) });

                PathNode currentNode = endNode;
                while (currentNode.PreviousNodeIndex != -1)
                {
                    currentNode = nodes[currentNode.PreviousNodeIndex];
                    buffer.Add(new() { GridPosition = new(currentNode.X, currentNode.Y) });
                }
            }
        }
    }
}
