
using Unity.Entities;
using Unity.Mathematics;

namespace Enemies
{
    [System.Serializable]
    public struct PathfindingParametersData : IComponentData
    {
        public int2 Start;
        public int2 End;
    }
}