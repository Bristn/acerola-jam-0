using Buildings.Base;
using Cameras.Targets;
using Common.Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TowerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerSystem: OnCreate");
            state.RequireForUpdate<TowerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            TowerSpawnerData towerSpawner = SystemAPI.GetSingleton<TowerSpawnerData>();
            RefRW<BaseData> baseDate = SystemAPI.GetSingletonRW<BaseData>();
            foreach (var (towerTransform, tower) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<TowerData>>())
            {
                // Determine if this tower can fire at an enemy
                bool canFire = tower.ValueRO.CanFire;
                if (!canFire)
                {
                    tower.ValueRW.ReduceFireCooldown(Time.deltaTime);
                }

                canFire = tower.ValueRO.CanFire && baseDate.ValueRO.AmmoResoruces > 0;
                if (!canFire)
                {
                    continue;
                }

                // Get the nearest enemy in range
                float2 towerPosition = new(towerTransform.ValueRO.Position.x, towerTransform.ValueRO.Position.y);
                float nearestDistance = float.MaxValue;
                float2 nearestPosition = new(float.MaxValue, float.MaxValue);
                foreach (var (enemyTransform, towerTarget, health, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<TowerTargetData>, RefRO<HealthData>>().WithEntityAccess())
                {
                    float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);
                    float distance = math.distance(towerPosition, enemyPosition);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestPosition = enemyPosition;
                    }
                }

                // Validate that there is a target
                if (nearestPosition.x == float.MaxValue || nearestDistance > tower.ValueRO.Radius)
                {
                    continue;
                }

                // Spawn the projectile entity
                Entity prefab = towerSpawner.ProjectilePrefab;
                NativeArray<Entity> instances = new(1, Allocator.Temp);
                commandBuffer.Instantiate(prefab, instances);

                float2 baseDirection = math.normalizesafe(nearestPosition - towerPosition);
                foreach (var projectile in instances)
                {
                    commandBuffer.AddComponent<TowerProjectileData>(projectile);
                    commandBuffer.SetComponent(projectile, new TowerProjectileData()
                    {
                        Damage = 50,
                        Direction = baseDirection,
                        Speed = 10f
                    });

                    commandBuffer.SetComponent(projectile, new LocalTransform()
                    {
                        Position = new(towerPosition.x, towerPosition.y, -5),
                        Scale = 0.2f,
                        Rotation = quaternion.LookRotationSafe(math.forward(), new(baseDirection.x, baseDirection.y, 0))
                    });
                }

                tower.ValueRW.CanFire = false;
                baseDate.ValueRW.AmmoResoruces--;
                instances.Dispose();
            }
        }
    }
}