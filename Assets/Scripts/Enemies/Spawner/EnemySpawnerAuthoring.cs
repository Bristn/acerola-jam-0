using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawnerAuthoring : MonoBehaviour
    {
        /* --- References --- */

        [SerializeField][BoxGroup("References")] private GameObject prefab;

        public class Baker : Baker<EnemySpawnerAuthoring>
        {
            public override void Bake(EnemySpawnerAuthoring authoring)
            {
                Debug.Log("EnemySpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new EnemySpawnerData
                {
                    Prefab = this.GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    MaxRandomOffset = 5,
                });

                this.AddBuffer<EnemiesToSpawnBuffer>(entity);
            }
        }
    }
}