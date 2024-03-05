using Buildings.Towers;
using Cameras.Targets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

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

            new KillEnemyJob()
            {
                CommandBuffer = commandBuffer.AsParallelWriter(),
            }.ScheduleParallel();
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
                        Scale = 1,
                    });
                }

                this.CommandBuffer.DestroyEntity(sortKey, entity);
                instances.Dispose();
            }
        }
    }
}