using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Common.Health
{
    public class HealthAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private float baseHealth;
        [SerializeField][BoxGroup("Settings")] private float currentHealth;

        public class Baker : Baker<HealthAuthoring>
        {
            public override void Bake(HealthAuthoring authoring)
            {
                Debug.Log("HealthAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new HealthData
                {
                    BaseHealth = authoring.baseHealth,
                    CurrentHealth = authoring.currentHealth,
                });
            }
        }
    }
}