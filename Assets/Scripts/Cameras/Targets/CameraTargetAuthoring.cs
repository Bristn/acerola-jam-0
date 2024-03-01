using Unity.Entities;
using UnityEngine;

namespace Cameras.Targets
{
    public class CameraTargetAuthoring : MonoBehaviour
    {
        public class Baker : Baker<CameraTargetAuthoring>
        {
            public override void Bake(CameraTargetAuthoring authoring)
            {
                Debug.Log("CameraTargetAuthoring: Bake");
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                this.AddComponent(entity, new CameraTargetData { });
            }
        }
    }
}