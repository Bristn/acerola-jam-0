
using Unity.Entities;
using Unity.Mathematics;

namespace Enemies
{
    [System.Serializable]
    public struct EnemySpawnerData : IComponentData
    {
        public Entity Prefab;
        public float MaxRandomOffset;
    }
}