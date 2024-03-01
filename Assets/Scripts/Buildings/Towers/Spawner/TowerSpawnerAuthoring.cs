using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class TowerSpawnerAuthoring : MonoBehaviour
    {
        /* --- References --- */

        [SerializeField][BoxGroup("References")] private GameObject prefab;

        public class Baker : Baker<TowerSpawnerAuthoring>
        {
            public override void Bake(TowerSpawnerAuthoring authoring)
            {
                Debug.Log("TowerSpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TowerSpawnerData
                {
                    Prefab = this.GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}