
using Unity.Entities;
using Unity.Mathematics;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerProjectileData : IComponentData
    {
        public float2 Origin;
        public float2 Direction;
        public float Speed;
        public float Damage;
    }
}