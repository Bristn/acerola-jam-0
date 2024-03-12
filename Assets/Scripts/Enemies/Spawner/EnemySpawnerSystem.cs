using System;
using Buildings.Base;
using Common;
using Pathfinding;
using Pathfinding.Followers;
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
                toSpawn.RemoveAtSwapBack(index);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
            indexToRemove.Dispose();
        }

        // TODO: Add passive enemy spawns with more random pathfinding
        private void SpawnSingleEnemy(EntityCommandBuffer commandBuffer, EnemySpawnerData spawner, float2 center)
        {
            // Tries to spawn the enemy 5 times
            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
            float2 offsetMin = new(-spawner.MaxRandomOffset, -spawner.MaxRandomOffset);
            float2 offsetMax = new(spawner.MaxRandomOffset, spawner.MaxRandomOffset);
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

                // Determine the path target
                TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();
                int2 targetCell = tilemapData.CenterCell;

                // Otherwise spawn enemy at this position & make it pathfind to the base
                Entity entity = commandBuffer.Instantiate(spawner.Prefab);
                commandBuffer.SetComponent(entity, new LocalTransform()
                {
                    Position = new float3(spawnPosition.x, spawnPosition.y, -5),
                    Scale = 0.5f
                });

                commandBuffer.AddComponent<PathfindingRequestPathData>(entity);
                commandBuffer.SetComponent(entity, new PathfindingRequestPathData()
                {
                    StartCell = new(cellIndex.x, cellIndex.y),
                    EndCell = targetCell,
                });

                return;
            }
        }
    }
}