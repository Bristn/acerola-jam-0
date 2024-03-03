using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.Positions
{
    [InternalBufferCapacity(20)]
    public partial struct PathPosition : IBufferElementData
    {
        public int2 GridPosition;
    }
}