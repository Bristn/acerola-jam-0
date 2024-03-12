
using Unity.Entities;
using Unity.Mathematics;

namespace Pathfinding.Followers
{
    [System.Serializable]
    public struct PathFollowerData : IComponentData
    {
        public int CurrentCellIndex;
        public float2 OffsetFromPath;
    }
}