
using Unity.Entities;

namespace Enemies
{
    [System.Serializable]
    public struct EnemyData : IComponentData
    {
        public Entity PickupPrefab;
    }
}