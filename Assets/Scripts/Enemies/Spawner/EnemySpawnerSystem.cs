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
    public partial class EnemySpawnerSystem : SystemBase
    {
        private bool isFirstWave;
        private Unity.Mathematics.Random random;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemySpawnerSystem: OnCreate");
            RequireForUpdate<EnemySpawnerData>();
            RequireForUpdate<EnemySpawnerEnableData>();
            RequireForUpdate<TilemapData>();

            this.random = Unity.Mathematics.Random.CreateFromIndex(0);
            this.isFirstWave = true;
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // Check if a new wave should be spawned
            RefRW<EnemySpawnerData> spawnerData = SystemAPI.GetSingletonRW<EnemySpawnerData>();
            if (!spawnerData.ValueRW.ReduceWaveCooldown(SystemAPI.Time.DeltaTime))
            {
                return;
            }

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();

            // Spawn first wave at a fices position & disable spawning afterwards to show a new dialog
            if (this.isFirstWave)
            {
                float2 fixedWavePosition = new(tilemapData.CenterOfGrid.x + 10, tilemapData.CenterOfGrid.y);
                this.SpawnEnemyWave(commandBuffer, spawnerData.ValueRO.Prefab, fixedWavePosition, tilemapData.CenterCell);

                foreach (var (_, entity) in SystemAPI.Query<RefRW<EnemySpawnerEnableData>>().WithEntityAccess())
                {
                    commandBuffer.DestroyEntity(entity);
                }

                this.isFirstWave = false;
                commandBuffer.Playback(EntityManager);
                commandBuffer.Dispose();
                return;
            }

            // Spawn a random wave
            float angle = random.NextFloat(0, Mathf.PI * 2);
            float radius = 10;

            float2 waveCenter = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            float2 enemyBasePosition = new(tilemapData.CenterOfGrid.x + waveCenter.x, tilemapData.CenterOfGrid.y + waveCenter.y);

            this.SpawnEnemyWave(commandBuffer, spawnerData.ValueRO.Prefab, enemyBasePosition, tilemapData.CenterCell);
            spawnerData.ValueRW.WaveCount++;

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        private void SpawnEnemyWave(EntityCommandBuffer commandBuffer, Entity prefab, float2 waveCenter, int2 pathTarget)
        {
            NativeArray<Entity> instances = new(1, Allocator.Temp);
            commandBuffer.Instantiate(prefab, instances);

            foreach (Entity entity in instances)
            {
                float2 enemyOffset = this.random.NextFloat2(-1, 1);
                float2 enemyPosition = new(waveCenter.x + enemyOffset.x, waveCenter.y + enemyOffset.y);
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
                    EndCell = pathTarget,
                });
            }

            instances.Dispose();
        }
    }
}