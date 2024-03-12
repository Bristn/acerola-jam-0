using Common;
using Common.Health;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]

    public partial struct TowerProjectileSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerProjectileSystem: OnCreate");
            state.RequireForUpdate<TowerProjectileData>();
            state.RequireForUpdate<ResumeTimeData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (projectileTransform, projectile, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TowerProjectileData>>().WithEntityAccess())
            {
                float2 moveBy = projectile.ValueRO.Direction * projectile.ValueRO.Speed * Time.deltaTime;
                projectileTransform.ValueRW.Position += new float3(moveBy.x, moveBy.y, 0);

                float2 projectilePosition = new(projectileTransform.ValueRO.Position.x, projectileTransform.ValueRO.Position.y);
                foreach (var (targetTransform, health, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<HealthData>, RefRW<TowerProjectileTargetData>>())
                {
                    float2 targetPosition = new(targetTransform.ValueRO.Position.x, targetTransform.ValueRO.Position.y);
                    float distance = math.distance(projectilePosition, targetPosition);

                    if (distance <= 0.3f)
                    {
                        health.ValueRW.CurrentHealth -= projectile.ValueRO.Damage;
                    }
                }

                // Destroy projectile once it travelled to far
                float travelledDistance = math.distance(projectilePosition, projectile.ValueRO.Origin);
                if (travelledDistance > 20)
                {
                    commandBuffer.DestroyEntity(entity);
                }
            }
        }
    }
}