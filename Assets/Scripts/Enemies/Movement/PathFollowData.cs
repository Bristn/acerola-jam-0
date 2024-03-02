
using Unity.Entities;
using Unity.Mathematics;

namespace Enemies
{
    [System.Serializable]
    public struct PathFollowData : IComponentData
    {
        public int Index;
    }
}