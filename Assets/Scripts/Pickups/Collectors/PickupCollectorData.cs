
using Unity.Entities;

namespace Pickups
{
    public struct PickupCollectorData : IComponentData
    {
        public int StoredPickups;
        public float PickupRadius;
    }
}