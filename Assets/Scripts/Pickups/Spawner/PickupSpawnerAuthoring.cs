using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Pickups
{
    public class PickupSpawnerAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private GameObject prefab;
        [SerializeField][BoxGroup("Settings")] private float randomRadius;

        public class Baker : Baker<PickupSpawnerAuthoring>
        {
            public override void Bake(PickupSpawnerAuthoring authoring)
            {
                Debug.Log("PickupSpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PickupSpawnerData
                {
                    Prefab = this.GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    RandomRadius = authoring.randomRadius,
                });
            }
        }
    }
}