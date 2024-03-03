using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileBaseLookup;

namespace Pathfinding.Algorithm
{
    public class PathHelpers
    {
        public static NativeArray<int2> EvenNeighborOffsets { get; private set; } = new(new int2[] {
            new(0, 1),    // Top-Right
            new(1, 0),    // Right
            new(0, -1),   // Bottom Right
            new(-1, -1),  // Bottom Left
            new(-1, 0),   // Left
            new(-1, 1),    // Top Left
        }, Allocator.Persistent);

        public static NativeArray<int2> OddNeighborOffsets { get; private set; } = new(new int2[] {
            new(1, 1),    // Top-Right
            new(1, 0),    // Right
            new(1, -1),   // Bottom Right
            new(0, -1),   // Bottom Left
            new(-1, 0),   // Left
            new(0, 1),    // Top Left
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
    }
}