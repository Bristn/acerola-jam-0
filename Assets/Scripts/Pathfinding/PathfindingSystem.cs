using Pathfinding.Algorithm;
using Pathfinding.Followers;
using Pathfinding.Positions;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Pathfinding
{
    public partial struct PathfindingSystem : ISystem
    {
        private NativeArray<int2> cellNeighborOffsets;
        private NativeArray<PathNode> allNodes;
        private int2 gridSize;

        public void OnCreate(ref SystemState state)
        {
            this.cellNeighborOffsets = PathHelpers.NeighborOffsets;

            this.gridSize = new(10, 10);
            this.allNodes = PathHelpers.GetNodesFromTilemap();
        }

        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            new FindPathJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
                CellNeighborOffsets = this.cellNeighborOffsets,
                GridSize = this.gridSize,
                // AllNodes = this.allNodes,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct FindPathJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public NativeArray<int2> CellNeighborOffsets; // Disable savety as access is readonly
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public int2 GridSize;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref PathfindingParametersData parameters, ref DynamicBuffer<PathPosition> buffer, ref PathFollowerData followData)
            {
                // TODO: Copy array which gets passed as parameter (Convert tilemap into basic pathnodes once and copy those for every iteration)
                NativeArray<PathNode> nodes = PathHelpers.GetNodesFromTilemap();

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
                    closed.Add(currentIndex);

                    Debug.Log(CellNeighborOffsets.Length);
                    for (int i = 0; i < CellNeighborOffsets.Length; i++)
                    {
                        int2 offset = CellNeighborOffsets[i];
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

                CommandBuffer.RemoveComponent<PathfindingParametersData>(sortKey, entity);
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
