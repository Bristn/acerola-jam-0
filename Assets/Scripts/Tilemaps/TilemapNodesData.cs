
using Pathfinding.Algorithm;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Tilemaps
{
    public partial struct TilemapNodesData : IBufferElementData
    {
        public PathNode Node;
    }
}