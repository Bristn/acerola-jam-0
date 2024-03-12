using Buildings.Base;
using Buildings.Towers;
using Cameras;
using Enemies;
using Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Common.Health
{
    public partial struct HealthSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("HealthSystem: OnCreate");
            state.RequireForUpdate<HealthData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            JobHandle enemyJob = new KillEnemyJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
            }.ScheduleParallel(new JobHandle());
            enemyJob.Complete();

            this.KillPlayer(ref state, commandBuffer);
        }

        private void KillPlayer(ref SystemState state, EntityCommandBuffer commandBuffer)
        {
            RefRW<BaseData> baseData = SystemAPI.GetSingletonRW<BaseData>();
            foreach (var (health, _, entity) in SystemAPI.Query<RefRW<HealthData>, RefRO<PlayerMovementData>>().WithEntityAccess())
            {
                if (health.ValueRO.CurrentHealth > 0)
                {
                    return;
                }

                commandBuffer.DestroyEntity(entity);

                // Adjust player lifes (Gameover detection happens in BaseSystem callbacks)
                baseData.ValueRW.PlayerLifes--;

                // Move camera to center
                Entity recenter = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<CameraRecenterData>(recenter);
                commandBuffer.SetComponent(recenter, new CameraRecenterData()
                {
                    Speed = 3f
                });
            }
        }

        [BurstCompile]
        private partial struct KillEnemyJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref HealthData health, in EnemyData enemy, in LocalTransform enemyTransform)
            {
                if (health.CurrentHealth > 0)
                {
                    return;
                }

                // Spawn loot
                NativeArray<Entity> instances = new(1, Allocator.Temp);
                this.CommandBuffer.Instantiate(sortKey, enemy.PickupPrefab, instances);

                foreach (var pickup in instances)
                {
                    this.CommandBuffer.SetComponent(sortKey, pickup, new LocalTransform()
                    {
                        Position = enemyTransform.Position,
                        Scale = 0.25f,
                    });
                }

                this.CommandBuffer.DestroyEntity(sortKey, entity);
                instances.Dispose();
            }
        }
    }
}