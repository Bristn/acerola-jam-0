using Unity.Collections;
using Unity.Mathematics;

namespace Pathfinding.Algorithm
{
    public class PathHelpers
    {
        public static NativeArray<int2> NeighborOffsets { get; private set; } = new(new int2[] {
            new(-1, 0), // Left
            new(1, 0),  // Right
            new(-1, 1), // Top Left
            new(0, 1),  // Top
            new(1, 1),  // Top Right
            new(-1, 1), // Bottom Right
            new(0, 1),  // Bottom
            new(-1, -1),// Bottom Left
        }, Allocator.Persistent);

        public static int GetCellIndex(int x, int y, int width)
        {
            return x + y * width;
        }

        public static bool IsValidCellIndex(int2 index, int2 gridSize)
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

        public static int GetHeuristicCost(int2 from, int2 to)
        {
            int xDistance = math.abs(from.x - to.x);
            int yDistance = math.abs(from.y - to.y);
            int remaining = math.abs(xDistance - yDistance);
            return math.min(xDistance, yDistance) + remaining;
        }

        public static int GetLowesetFCostNodeIndex(NativeList<int> open, NativeArray<PathNode> nodes)
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

        public static NativeArray<PathNode> GetNodesFromTilemap()
        {
            int2 gridSize = new(10, 10);

            NativeArray<PathNode> nodes = new(gridSize.x * gridSize.y, Allocator.Temp);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    PathNode node = new()
                    {
                        X = x,
                        Y = y,
                        Index = GetCellIndex(x, y, gridSize.x),
                        GCost = int.MaxValue,
                        IsWalkable = true, // TODO: Set
                        PreviousNodeIndex = -1
                    };

                    node.UpdateFCost();
                    nodes[node.Index] = node;
                }
            }

            return nodes;
        }
    }
}