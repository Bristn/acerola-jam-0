using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Pathfinding.Followers
{
    public class PathFollowerAuthoring : MonoBehaviour
    {
        /* --- Settings --- */

        [SerializeField][BoxGroup("Settings")] private int currentCellIndex;
        [SerializeField][BoxGroup("Settings")] private float regularSpeed;
        [SerializeField][BoxGroup("Settings")] private float slowSpeed;

        public class Baker : Baker<PathFollowerAuthoring>
        {
            public override void Bake(PathFollowerAuthoring authoring)
            {
                Debug.Log("PathFollowerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PathFollowerData
                {
                    CurrentCellIndex = authoring.currentCellIndex,
                    RegularSpeed = authoring.regularSpeed,
                    SlowSpeed = authoring.slowSpeed,
                });
            }
        }
    }
}