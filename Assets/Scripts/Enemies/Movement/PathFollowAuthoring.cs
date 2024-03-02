using Enemies;
using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class PathFollowAuthoring : MonoBehaviour
    {
        public int Index;

        public class Baker : Baker<PathFollowAuthoring>
        {
            public override void Bake(PathFollowAuthoring authoring)
            {
                Debug.Log("PathFollowAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PathFollowData
                {
                    Index = authoring.Index
                });
            }
        }
    }
}