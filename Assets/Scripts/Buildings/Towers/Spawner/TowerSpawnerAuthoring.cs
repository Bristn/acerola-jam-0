using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class TowerSpawnerAuthoring : MonoBehaviour
    {
        /* --- References --- */

        [SerializeField][BoxGroup("References")] private GameObject towerPrefab;
        [SerializeField][BoxGroup("References")] private GameObject projectilePrefab;

        public class Baker : Baker<TowerSpawnerAuthoring>
        {
            public override void Bake(TowerSpawnerAuthoring authoring)
            {
                Debug.Log("TowerSpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TowerSpawnerData
                {
                    TowerPrefab = this.GetEntity(authoring.towerPrefab, TransformUsageFlags.Dynamic),
                    ProjectilePrefab = this.GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}