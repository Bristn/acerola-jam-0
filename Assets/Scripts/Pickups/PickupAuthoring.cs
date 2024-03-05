using Unity.Entities;
using UnityEngine;

namespace Pickups
{
    public class PickupAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PickupAuthoring>
        {
            public override void Bake(PickupAuthoring authoring)
            {
                Debug.Log("PickupAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PickupData { });
            }
        }
    }
}