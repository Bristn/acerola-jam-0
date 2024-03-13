
using Unity.Entities;
using Unity.Mathematics;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerTargetVisualiserData : IComponentData
    {
        public int TowerIndex;
        public float3 Position;
    }
}