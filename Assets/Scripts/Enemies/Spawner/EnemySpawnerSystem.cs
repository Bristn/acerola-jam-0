using System;
using Buildings.Base;
using Pathfinding;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies
{
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("EnemySpawnerSystem: OnCreate");
            state.RequireForUpdate<EnemySpawnerData>();
            state.RequireForUpdate<TilemapData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            // Check if a new wave should be spawned
            RefRW<EnemySpawnerData> spawnerData = SystemAPI.GetSingletonRW<EnemySpawnerData>();
            if (!spawnerData.ValueRW.ReduceWaveCooldown(Time.deltaTime))
            {
                return;
            }

            EntityManager entityManager = state.EntityManager;

            // Get the spawn center
            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();

            Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex(spawnerData.ValueRO.WaveCount);
            float angle = random.NextFloat(0, Mathf.PI * 2);
            float radius = 10;

            float2 waveCenter = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            float2 enemyBasePosition = new(tilemapData.CenterOfGrid.x + waveCenter.x, tilemapData.CenterOfGrid.y + waveCenter.y);

            // Instantiate prefab
            Entity prefab = spawnerData.ValueRO.Prefab;
            NativeArray<Entity> instances = new(1, Allocator.Temp);
            commandBuffer.Instantiate(prefab, instances);


            foreach (Entity entity in instances)
            {
                float2 enemyOffset = random.NextFloat2(-1, 1);
                float2 enemyPosition = new(enemyBasePosition.x + enemyOffset.x, enemyBasePosition.y + enemyOffset.y);
                Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
                Vector3Int cellIndex = tilemap.WorldToCell(new(enemyPosition.x, enemyPosition.y, 0));

                commandBuffer.SetComponent(entity, new LocalTransform()
                {
                    Position = new float3(enemyPosition.x, enemyPosition.y, -5),
                    Scale = 0.5f
                });

                commandBuffer.AddComponent<PathfindingRequestPathData>(entity);
                commandBuffer.SetComponent(entity, new PathfindingRequestPathData()
                {
                    StartCell = new(cellIndex.x, cellIndex.y),
                    EndCell = tilemapData.CenterCell,
                });
            }

            instances.Dispose();
        }
    }
}