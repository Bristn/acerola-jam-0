using Unity.Entities;
using UnityEngine;

namespace Tilemaps
{
    public class TilemapAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TilemapAuthoring>
        {
            public override void Bake(TilemapAuthoring authoring)
            {
                Debug.Log("TilemapAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TilemapData { });
                this.AddBuffer<TilemapNodesData>(entity);
            }
        }
    }
}