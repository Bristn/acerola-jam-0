
using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding
{
    [System.Serializable]
    public struct PathfindingParametersData : IComponentData
    {
        public int2 StartCell;
        public int2 EndCell;
    }
}