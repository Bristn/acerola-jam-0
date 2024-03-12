using System;
using Cameras.Targets;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Cameras
{
    public partial class CameraRecenterSystem : SystemBase
    {
        public static Action ReachedCenter;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("CameraRecenterSystem: OnCreate");
            RequireForUpdate<TilemapData>();
            RequireForUpdate<CameraRecenterData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            // If there is a CameraTargetData, remove its entity
            foreach (var (_, entity) in SystemAPI.Query<RefRO<CameraTargetData>>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(entity);
            }

            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();
            float2 targetPosition = tilemapData.CenterOfGrid;
            float2 cameraPosition = new(GameObjectLocator.Instance.MainCamera.transform.position.x, GameObjectLocator.Instance.MainCamera.transform.position.y);

            float2 direction = math.normalize(targetPosition - cameraPosition);
            float totalDistance = math.distance(targetPosition, cameraPosition);

            foreach (var (recenter, entity) in SystemAPI.Query<RefRO<CameraRecenterData>>().WithEntityAccess())
            {
                float maxDistanceThisFrame = recenter.ValueRO.Speed * SystemAPI.Time.DeltaTime;
                if (totalDistance > maxDistanceThisFrame)
                {
                    cameraPosition += direction * maxDistanceThisFrame;
                    continue;
                }

                cameraPosition += direction * totalDistance;
                if (totalDistance < 0.1f)
                {
                    ReachedCenter?.Invoke();
                    commandBuffer.DestroyEntity(entity);
                }
            }

            if (!float.IsNaN(cameraPosition.x) && !float.IsNaN(cameraPosition.y))
            {
                GameObjectLocator.Instance.MainCamera.transform.position = new(cameraPosition.x, cameraPosition.y, GameObjectLocator.Instance.MainCamera.transform.position.z);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}