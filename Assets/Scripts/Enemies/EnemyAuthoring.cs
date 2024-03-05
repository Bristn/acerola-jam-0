using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Buildings.Towers
{
    public class EnemyAuthoring : MonoBehaviour
    {
        /* --- References --- */

        [SerializeField][BoxGroup("References")] private GameObject pickupPrefab;

        public class Baker : Baker<EnemyAuthoring>
        {
            public override void Bake(EnemyAuthoring authoring)
            {
                Debug.Log("EnemyAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new EnemyData
                {
                    PickupPrefab = this.GetEntity(authoring.pickupPrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}