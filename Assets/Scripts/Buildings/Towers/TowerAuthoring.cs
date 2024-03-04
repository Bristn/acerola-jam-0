using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Buildings.Towers
{
    public class TowerAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private GameObject projectilePrefab;
        [SerializeField][BoxGroup("Settings")] private float radius;
        [SerializeField][BoxGroup("Settings")] private float cooldown;

        public class Baker : Baker<TowerAuthoring>
        {
            public override void Bake(TowerAuthoring authoring)
            {
                Debug.Log("TowerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TowerData
                {
                    ProjectilePrefab = this.GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                    Radius = authoring.radius,
                    TotalFireCooldown = authoring.cooldown,
                    CurrentFireCooldown = authoring.cooldown,
                });
            }
        }
    }
}