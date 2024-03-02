using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class PathPositionAuthoring : MonoBehaviour
    {
        public class Baker : Baker<PathPositionAuthoring>
        {
            public override void Bake(PathPositionAuthoring authoring)
            {
                Debug.Log("PathPositionAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddBuffer<PathPosition>(entity);
            }
        }
    }
}