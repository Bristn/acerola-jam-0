
using Unity.Entities;

namespace Pathfinding.Followers
{
    [System.Serializable]
    public struct PathFollowerData : IComponentData
    {
        public int CurrentCellIndex;
    }
}