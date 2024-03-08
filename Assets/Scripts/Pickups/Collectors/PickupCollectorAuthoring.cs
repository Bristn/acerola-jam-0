using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Pickups
{
    public class PickupCollectorAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private float pickupRadius;
        [SerializeField][BoxGroup("Settings")] private int maxLoot;

        public class Baker : Baker<PickupCollectorAuthoring>
        {
            public override void Bake(PickupCollectorAuthoring authoring)
            {
                Debug.Log("PickupCollectorAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PickupCollectorData
                {
                    PickupRadius = authoring.pickupRadius,
                    MaxPickups = authoring.maxLoot,
                });
            }
        }
    }
}