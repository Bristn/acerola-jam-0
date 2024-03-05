using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    public class TowerProjectileTargetAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TowerProjectileTargetAuthoring>
        {
            public override void Bake(TowerProjectileTargetAuthoring authoring)
            {
                Debug.Log("TowerProjectileTargetAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TowerProjectileTargetData
                {
                });
            }
        }
    }
}