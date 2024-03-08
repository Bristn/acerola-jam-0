
using Unity.Entities;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct TowerSpawnerData : IComponentData
    {
        public Entity TowerPrefab;
        public Entity ProjectilePrefab;
    }
}