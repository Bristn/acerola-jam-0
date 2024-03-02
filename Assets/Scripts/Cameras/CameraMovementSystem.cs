using Cameras.Targets;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Cameras
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class CameraMovementSystem : SystemBase
    {
        private float3 previousPosition = new(0, 0, -10);
        private float zoom = 5;

        [BurstCompile]
        protected override void OnUpdate()
        {
            float3 currentPosition = this.previousPosition;

            // Get the position of the target to follor
            foreach (var (transform, target) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<CameraTargetData>>())
            {
                currentPosition.x = transform.ValueRO.Position.x;
                currentPosition.y = transform.ValueRO.Position.y;
            }

            GameObjectLocator.Instance.MainCamera.transform.position = currentPosition;
            this.previousPosition = currentPosition;

            // Zooming is realised with the orthographic scale of the camera
            this.zoom += Input.mouseScrollDelta.y * -0.1f;
            this.zoom = Mathf.Clamp(this.zoom, 2.5f, 7.5f);
            GameObjectLocator.Instance.MainCamera.orthographicSize = this.zoom;
        }
    }
}