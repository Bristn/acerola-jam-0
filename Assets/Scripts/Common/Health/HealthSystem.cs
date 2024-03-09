using Buildings.Towers;
using Cameras.Targets;
using Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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

            JobHandle playerJob = new KillPlayerJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
            }.ScheduleParallel(enemyJob);
            playerJob.Complete();
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

        [BurstCompile]
        private partial struct KillPlayerJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref HealthData health, in PlayerMovementData player, in LocalTransform playerTransform)
            {
                if (health.CurrentHealth > 0)
                {
                    return;
                }

                // TODO: Move camera to center (gradual movement), reduce lifes & teleport player & show dialog

                this.CommandBuffer.DestroyEntity(sortKey, entity);
            }
        }
    }
}