using Buildings.Base;
using Common;
using Common.Health;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct TowerSystem : ISystem
    {
        /* --- Values --- */

        private Unity.Mathematics.Random random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerSystem: OnCreate");
            state.RequireForUpdate<TowerData>();
            state.RequireForUpdate<ResumeTimeData>();
            this.random = Unity.Mathematics.Random.CreateFromIndex(0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            TowerSpawnerData towerSpawner = SystemAPI.GetSingleton<TowerSpawnerData>();
            RefRW<BaseData> baseData = SystemAPI.GetSingletonRW<BaseData>();
            foreach (var (towerTransform, tower) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<TowerData>>())
            {
                // Determine if this tower can fire at an enemy
                bool canFire = tower.ValueRO.CanFire;
                if (!canFire)
                {
                    tower.ValueRW.ReduceFireCooldown(Time.deltaTime);
                }

                // Update target visualiser
                float2 towerPosition = new(towerTransform.ValueRO.Position.x, towerTransform.ValueRO.Position.y);
                TargetData targetData = this.GetNearestEnemyEntity(ref state, towerPosition);
                if (!targetData.HasTarget || targetData.Distance > tower.ValueRO.Radius)
                {
                    this.UpdateTowerTargetVisualiser(ref state, tower.ValueRO.Index, new(0, 0, 100));
                    continue;
                }

                this.UpdateTowerTargetVisualiser(ref state, tower.ValueRO.Index, new(targetData.Position.x, targetData.Position.y, -6));

                canFire = tower.ValueRO.CanFire && baseData.ValueRO.AmmoResoruces > 0;
                if (!canFire)
                {
                    continue;
                }

                // Spawn the projectile entity
                this.SpawnProjectiles(commandBuffer, tower.ValueRO, towerSpawner.ProjectilePrefab, targetData.Position, towerPosition);
                tower.ValueRW.CanFire = false;
                baseData.ValueRW.AmmoResoruces -= 1;
            }
        }

        private struct TargetData
        {
            public Entity Entity;
            public float Distance;
            public float2 Position;
            public bool HasTarget;
        }

        private TargetData GetNearestEnemyEntity(ref SystemState state, float2 towerPosition)
        {
            float nearestDistance = float.MaxValue;
            Entity nearestEntity = default;
            float2 nearestPosition = new(float.MaxValue, float.MaxValue);
            foreach (var (enemyTransform, towerTarget, health, enemyEntity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<TowerTargetData>, RefRO<HealthData>>().WithEntityAccess())
            {
                float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);
                float distance = math.distance(towerPosition, enemyPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEntity = enemyEntity;
                    nearestPosition = enemyPosition;
                }
            }

            if (nearestDistance == float.MaxValue)
            {
                return new()
                {
                    HasTarget = false,
                };
            }

            return new()
            {
                Entity = nearestEntity,
                Distance = nearestDistance,
                HasTarget = true,
                Position = nearestPosition,
            };
        }

        private void UpdateTowerTargetVisualiser(ref SystemState state, int towerIndex, float3 targetPosition)
        {
            foreach (var visualiser in SystemAPI.Query<RefRW<TowerTargetVisualiserData>>())
            {
                if (visualiser.ValueRO.TowerIndex != towerIndex)
                {
                    continue;
                }

                visualiser.ValueRW.Position = targetPosition;
                return;
            }
        }

        private void SpawnProjectiles(EntityCommandBuffer commandBuffer, TowerData towerData, Entity prefab, float2 targetPosition, float2 towerPosition)
        {
            NativeArray<Entity> instances = new(towerData.BulletCountPerShot, Allocator.Temp);
            commandBuffer.Instantiate(prefab, instances);

            foreach (var projectile in instances)
            {
                float2 offset = new(
                    random.NextFloat(-towerData.BulletRandomness, towerData.BulletRandomness),
                    random.NextFloat(-towerData.BulletRandomness, towerData.BulletRandomness)
                );

                float2 direction = math.normalizesafe(targetPosition + offset - towerPosition);

                commandBuffer.AddComponent<TowerProjectileData>(projectile);
                commandBuffer.SetComponent(projectile, new TowerProjectileData()
                {
                    Damage = 1,
                    Direction = direction,
                    Speed = towerData.BulletVelocity,
                    Origin = towerPosition,
                });

                commandBuffer.SetComponent(projectile, new LocalTransform()
                {
                    Position = new(towerPosition.x, towerPosition.y, -5),
                    Scale = 0.2f,
                    Rotation = quaternion.LookRotationSafe(math.forward(), new(direction.x, direction.y, 0))
                });
            }

            instances.Dispose();
        }
    }
}