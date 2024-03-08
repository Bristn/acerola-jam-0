using NaughtyAttributes;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class TowerPlacemementAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TowerPlacemementAuthoring>
        {
            public override void Bake(TowerPlacemementAuthoring authoring)
            {
                Debug.Log("TowerPlacememenAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                this.AddComponent(entity, new TowerPlacemementData() { });
            }
        }
    }
}