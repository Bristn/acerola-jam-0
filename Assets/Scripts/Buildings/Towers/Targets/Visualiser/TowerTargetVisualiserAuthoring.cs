using Unity.Entities;
using UnityEngine;

namespace Buildings.Towers
{
    public class TowerTargetVisualiserAuthoring : MonoBehaviour
    {
        public class Baker : Baker<TowerTargetVisualiserAuthoring>
        {
            public override void Bake(TowerTargetVisualiserAuthoring authoring)
            {
                Debug.Log("TowerTargetVisualiserAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                this.AddComponent(entity, new TowerTargetVisualiserData
                {
                    TowerIndex = -1,
                });
            }
        }
    }
}