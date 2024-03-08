
using Unity.Entities;
using Unity.Mathematics;

namespace Buildings
{
    [System.Serializable]
    public struct BuildingData : IComponentData
    {
        public int2 Index;
    }
}