using System;
using Buildings.Base;
using Common;
using Pathfinding;
using Pathfinding.Followers;
using Players;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileBaseLookup;

namespace Enemies
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EnemySpawnerSystem : SystemBase
    {
        private Unity.Mathematics.Random random;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemySpawnerSystem: OnCreate");
            RequireForUpdate<EnemySpawnerData>();
            RequireForUpdate<EnemiesToSpawnBuffer>();

            RequireForUpdate<TilemapData>();
            RequireForUpdate<ResumeTimeData>();
            this.random = Unity.Mathematics.Random.CreateFromIndex(0);
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            NativeList<int> indexToRemove = new(Allocator.Temp);

            EnemySpawnerData spawner = SystemAPI.GetSingleton<EnemySpawnerData>();
            DynamicBuffer<EnemiesToSpawnBuffer> toSpawn = SystemAPI.GetSingletonBuffer<EnemiesToSpawnBuffer>();
            for (int i = 0; i < toSpawn.Length; i++)
            {
                EnemiesToSpawnBuffer element = toSpawn.ElementAt(i);
                element.Delay -= SystemAPI.Time.DeltaTime;

                if (element.Delay <= 0)
                {
                    this.SpawnSingleEnemy(commandBuffer, spawner, element.Center);
                    indexToRemove.Add(i);
                }

                toSpawn[i] = element;
            }

            // Remove already spawned enemies
            foreach (int index in indexToRemove)
            {
                try
                {
                    toSpawn.RemoveAtSwapBack(index);
                }
                catch (System.Exception) { }
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            indexToRemove.Dispose();
        }

        private void SpawnSingleEnemy(EntityCommandBuffer commandBuffer, EnemySpawnerData spawner, float2 center)
        {
            // Tries to spawn the enemy 5 times
            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
            float2 offsetMin = new(-Helpers.EnemySpawnRandomness, -Helpers.EnemySpawnRandomness);
            float2 offsetMax = new(Helpers.EnemySpawnRandomness, Helpers.EnemySpawnRandomness);
            float2 playerPosition = this.GetPlayerPosition();

            for (int i = 0; i < 5; i++)
            {
                // Get random spawn position
                float2 offset = this.random.NextFloat2(offsetMin, offsetMax);
                float2 spawnPosition = center + offset;

                // Is spawn position valid (is it walkable)
                Vector3Int cellIndex = tilemap.WorldToCell(new(spawnPosition.x, spawnPosition.y, 0));
                TileType tileType = TileBaseLookup.Instance.GetTileType(cellIndex.x, cellIndex.y);
                bool isValid = TileBaseLookup.Instance.IsWalkable(tileType);

                // If not, try another position
                if (!isValid)
                {
                    continue;
                }

                // Checkk if the player is too close to this spawn point (Tries to reduce instant kill potential)
                float distanceToPlayer = math.distance(spawnPosition, playerPosition);
                if (distanceToPlayer <= 5)
                {
                    continue;
                }

                // Otherwise spawn enemy at this position & make it pathfind to the base
                Entity entity = commandBuffer.Instantiate(spawner.Prefab);
                commandBuffer.SetComponent(entity, new LocalTransform()
                {
                    Position = new float3(spawnPosition.x, spawnPosition.y, -5),
                    Scale = 0.5f
                });

                return;
            }

            Debug.LogError("Failed to spawn enemy");
        }


        private float2 GetPlayerPosition()
        {
            foreach (var (playerTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerMovementData>>())
            {
                return new(playerTransform.ValueRO.Position.x, playerTransform.ValueRO.Position.y);
            }

            return new(float.MaxValue, float.MaxValue);
        }
    }
}