
using Unity.Entities;

namespace Buildings.Towers
{
    [System.Serializable]
    public struct EnemyData : IComponentData
    {
        public Entity PickupPrefab;
    }
}