using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Enemies
{
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("EnemySpawnerSystem: OnCreate");
            state.RequireForUpdate<EnemySpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Check if a new wave should be spawned
            RefRW<EnemySpawnerData> spawnerData = SystemAPI.GetSingletonRW<EnemySpawnerData>();
            if (!spawnerData.ValueRW.ReduceWaveCooldown(Time.deltaTime))
            {
                return;
            }

            EntityManager entityManager = state.EntityManager;

            // Get the spawn center
            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex(spawnerData.ValueRO.WaveCount);
            float angle = random.NextFloat(0, Mathf.PI * 2);
            float radius = 10;
            float2 waveCenter = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);

            // Instantiate prefab
            Entity prefab = spawnerData.ValueRO.Prefab;
            NativeArray<Entity> instances = entityManager.Instantiate(prefab, 10, Allocator.Temp);
            foreach (Entity entity in instances)
            {
                float2 enemyOffset = random.NextFloat2(-1, 1);

                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = new float3(waveCenter.x + enemyOffset.x, waveCenter.y + enemyOffset.y, -5);
            }

            // Add Components to new instance
            /*
            entityManager.AddComponent<BuildingData>(instance);
            entityManager.SetComponentData(instance, new BuildingData()
            {
                Index = cellData.Index
            });
            */

            // Position prefab
            // RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(instance);
        }

        private void SpawnWave(float3 centerOfWave)
        {

        }
    }
}