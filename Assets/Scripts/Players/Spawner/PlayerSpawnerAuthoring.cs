using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Players
{
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        /* --- References --- */

        [SerializeField][BoxGroup("References")] private GameObject prefab;

        public class Baker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                Debug.Log("PlayerSpawnerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PlayerSpawnerData
                {
                    Prefab = this.GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                });
            }
        }
    }
}