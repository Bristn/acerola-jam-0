
using Pathfinding.Algorithm;
using Unity.Entities;
using Unity.Mathematics;

namespace Tilemaps
{
    public partial struct TilemapNodesData : IBufferElementData
    {
        public PathNode Node;
        public float2 WorldPosition;
    }
}