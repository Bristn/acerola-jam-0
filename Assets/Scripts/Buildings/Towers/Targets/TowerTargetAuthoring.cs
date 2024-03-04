using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

namespace Buildings.Towers
{
    public class TowerTargetAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TowerTargetAuthoring>
        {
            public override void Bake(TowerTargetAuthoring authoring)
            {
                Debug.Log("TowerTargetAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.None);

                this.AddComponent(entity, new TowerTargetData
                {
                });
            }
        }
    }
}