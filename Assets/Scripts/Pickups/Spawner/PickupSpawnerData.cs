
using Unity.Entities;

namespace Pickups
{
    public struct PickupSpawnerData : IComponentData
    {
        public Entity Prefab;
        public float RandomRadius;
    }
}