
using Unity.Entities;
using Unity.Mathematics;

namespace Tilemaps
{
    public struct TilemapData : IComponentData
    {
        public bool IsUpdated;
        public int2 GridSize;
        public float2 CenterOfGrid;
        public int2 CenterCell;
    }
}