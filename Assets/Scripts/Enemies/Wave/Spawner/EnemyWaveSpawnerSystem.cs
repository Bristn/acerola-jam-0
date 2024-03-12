using Common;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Enemies
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EnemyWaveSpawnerSystem : SystemBase
    {
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
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
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
                this.SpawnEnemyWave(spawner.ValueRO, fixedWavePosition);

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
            this.SpawnEnemyWave(spawner.ValueRO, waveCenter);
        }

        /// <summary>
        /// Writes the spawn information to a dynamic buffer which will be handled by the actual EnemySpawnerSystem
        /// </summary>
        private void SpawnEnemyWave(EnemyWaveSpawnerData spawner, float2 center)
        {
            DynamicBuffer<EnemiesToSpawnBuffer> toSpawn = SystemAPI.GetSingletonBuffer<EnemiesToSpawnBuffer>();

            for (int i = 0; i < spawner.EnemiesPerWave; i++)
            {
                toSpawn.Add(new EnemiesToSpawnBuffer()
                {
                    Center = center,
                    Delay = this.random.NextFloat(spawner.MaxEnemySpawnDelay)
                });
            }
        }
    }
}