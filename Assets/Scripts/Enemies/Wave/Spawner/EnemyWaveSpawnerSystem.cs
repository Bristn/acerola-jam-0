using System;
using Common;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enemies
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EnemyWaveSpawnerSystem : SystemBase
    {
        private float currentPassiveSpawnDelay;
        private int currentPassiveSpawn;
        private int maxPassiveSpawn;

        private bool isFirstWave;
        private Unity.Mathematics.Random random;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemyWaveSpawnerSystem: OnCreate");
            RequireForUpdate<EnemyWaveSpawnerData>();
            RequireForUpdate<EnemyWaveSpawnerEnableData>();
            RequireForUpdate<EnemiesToSpawnBuffer>();
            RequireForUpdate<TilemapData>();
            RequireForUpdate<ResumeTimeData>();

            this.random = Unity.Mathematics.Random.CreateFromIndex(0);
            this.isFirstWave = true;
            this.currentPassiveSpawnDelay = 0;
            this.maxPassiveSpawn = 24;
            this.currentPassiveSpawn = 0;
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            if (!this.isFirstWave)
            {
                this.QueuePassiveSpawns();
            }

            // Check if a new wave should be spawned
            RefRW<EnemyWaveSpawnerData> spawner = SystemAPI.GetSingletonRW<EnemyWaveSpawnerData>();
            if (!spawner.ValueRW.ReduceWaveCooldown(SystemAPI.Time.DeltaTime))
            {
                return;
            }

            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();

            // Spawn first wave at a fices position & disable spawning afterwards to show a new dialog
            if (this.isFirstWave)
            {
                float2 fixedWavePosition = new(tilemapData.CenterOfGrid.x + Helpers.EnemySpawnDistance, tilemapData.CenterOfGrid.y);
                this.SpawnEnemyWave(fixedWavePosition, spawner.ValueRO.EnemiesPerWave, spawner.ValueRO.MaxEnemySpawnDelay);

                EntityCommandBuffer commandBuffer = new(Allocator.Temp);
                foreach (var (_, entity) in SystemAPI.Query<RefRW<EnemyWaveSpawnerEnableData>>().WithEntityAccess())
                {
                    commandBuffer.DestroyEntity(entity);
                }

                this.isFirstWave = false;
                commandBuffer.Playback(EntityManager);
                commandBuffer.Dispose();
                return;
            }

            // Determine the wave center
            float angle = this.random.NextFloat(0, Mathf.PI * 2);
            float2 waveOffset = new(Mathf.Cos(angle) * Helpers.EnemySpawnDistance, Mathf.Sin(angle) * Helpers.EnemySpawnDistance);
            float2 waveCenter = new(tilemapData.CenterOfGrid.x + waveOffset.x, tilemapData.CenterOfGrid.y + waveOffset.y);

            // Spawn the wave & adjust settings for next wave
            this.SpawnEnemyWave(waveCenter, spawner.ValueRO.EnemiesPerWave, spawner.ValueRO.MaxEnemySpawnDelay);
        }

        private void QueuePassiveSpawns()
        {
            if (this.currentPassiveSpawn >= this.maxPassiveSpawn)
            {
                return;
            }

            // Only queue spawns every 3 seconds
            this.currentPassiveSpawnDelay += SystemAPI.Time.DeltaTime;
            if (this.currentPassiveSpawnDelay < 1f)
            {
                return;
            }

            this.currentPassiveSpawnDelay = 0;
            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();

            // Determine the wave center
            float angleStep = math.PI2 / this.maxPassiveSpawn;
            float angle = angleStep * this.currentPassiveSpawn;

            float threshold = Helpers.EnemySpawnDistance + Helpers.EnemySpawnRandomness * 3f;
            float2 waveOffset = new(Mathf.Cos(angle) * threshold, Mathf.Sin(angle) * threshold);
            float2 waveCenter = new(tilemapData.CenterOfGrid.x + waveOffset.x, tilemapData.CenterOfGrid.y + waveOffset.y);

            this.SpawnEnemyWave(waveCenter, 10, 3);
            this.currentPassiveSpawn++;
        }

        /// <summary>
        /// Writes the spawn information to a dynamic buffer which will be handled by the actual EnemySpawnerSystem
        /// </summary>
        private void SpawnEnemyWave(float2 center, uint enemyCount, float maxSpawnDelay)
        {
            DynamicBuffer<EnemiesToSpawnBuffer> toSpawn = SystemAPI.GetSingletonBuffer<EnemiesToSpawnBuffer>();

            for (int i = 0; i < enemyCount; i++)
            {
                toSpawn.Add(new EnemiesToSpawnBuffer()
                {
                    Center = center,
                    Delay = this.random.NextFloat(maxSpawnDelay)
                });
            }
        }
    }
}