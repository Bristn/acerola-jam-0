using Buildings.Base;
using Buildings.Towers;
using Cameras;
using Enemies;
using Pickups;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Common.Health
{
    public partial struct HealthSystem : ISystem
    {
        private Unity.Mathematics.Random random;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("HealthSystem: OnCreate");
            state.RequireForUpdate<HealthData>();
            this.random = Unity.Mathematics.Random.CreateFromIndex(10);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            this.KillEnemy(ref state, commandBuffer);
            this.KillPlayer(ref state, commandBuffer);

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }

        private void KillPlayer(ref SystemState state, EntityCommandBuffer commandBuffer)
        {
            RefRW<BaseData> baseData = SystemAPI.GetSingletonRW<BaseData>();
            foreach (var (playerTransform, health, collector, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<HealthData>, RefRO<PickupCollectorData>>().WithEntityAccess())
            {
                if (health.ValueRO.CurrentHealth > 0)
                {
                    continue;
                }

                // Move camera to center
                Entity recenter = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<CameraRecenterData>(recenter);
                commandBuffer.SetComponent(recenter, new CameraRecenterData()
                {
                    Speed = 4f
                });

                // Spawn loot around player position
                for (int i = 0; i < collector.ValueRO.StoredPickups; i++)
                {
                    this.SpawnPickup(ref state, commandBuffer, playerTransform.ValueRO.Position, 3);
                }

                // Adjust player lifes (Gameover detection happens in BaseSystem callbacks)
                baseData.ValueRW.PlayerLifes--;

                commandBuffer.DestroyEntity(entity);
            }
        }

        private void KillEnemy(ref SystemState state, EntityCommandBuffer commandBuffer)
        {
            foreach (var (enemyTransform, health, _, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<HealthData>, RefRO<EnemyData>>().WithEntityAccess())
            {
                if (health.ValueRO.CurrentHealth > 0)
                {
                    continue;
                }

                this.SpawnPickup(ref state, commandBuffer, enemyTransform.ValueRO.Position);
                commandBuffer.DestroyEntity(entity);
            }
        }

        private void SpawnPickup(ref SystemState state, EntityCommandBuffer commandBuffer, float3 position, float randomMult = 1)
        {
            PickupSpawnerData spawner = SystemAPI.GetSingleton<PickupSpawnerData>();
            float3 min = new(-spawner.RandomRadius, -spawner.RandomRadius, 0);
            float3 max = new(spawner.RandomRadius, spawner.RandomRadius, 0);
            float3 offset = this.random.NextFloat3(min, max) * randomMult;

            NativeArray<Entity> instances = new(1, Allocator.Temp);
            commandBuffer.Instantiate(spawner.Prefab, instances);

            foreach (var pickup in instances)
            {
                commandBuffer.SetComponent(pickup, new LocalTransform()
                {
                    Position = position + offset,
                    Scale = 0.25f,
                });
            }

            instances.Dispose();
        }
    }
}