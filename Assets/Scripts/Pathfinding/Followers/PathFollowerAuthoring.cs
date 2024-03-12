using Unity.Entities;
using UnityEngine;

namespace Pathfinding.Followers
{
    public class PathFollowerAuthoring : MonoBehaviour
    {
        [SerializeField] private int CurrentCellIndex;

        public class Baker : Baker<PathFollowerAuthoring>
        {
            public override void Bake(PathFollowerAuthoring authoring)
            {
                Debug.Log("PathFollowerAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new PathFollowerData
                {
                    CurrentCellIndex = authoring.CurrentCellIndex,
                });
            }
        }
    }
}