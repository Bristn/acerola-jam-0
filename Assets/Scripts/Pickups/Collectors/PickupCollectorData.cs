
using Unity.Entities;

namespace Pickups
{
    public struct PickupCollectorData : IComponentData
    {
        public int MaxPickups;
        public int StoredPickups;
        public float PickupRadius;
    }
}