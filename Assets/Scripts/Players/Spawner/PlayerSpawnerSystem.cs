using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Players
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class PlayerSpawnerSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("PlayerSpawnerSystem: OnCreate");
            RequireForUpdate<PlayerSpawnerData>();
            RequireForUpdate<PlayerSpawnerEnableData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            // Spawn player at base & remove enable component
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            PlayerSpawnerData spawner = SystemAPI.GetSingleton<PlayerSpawnerData>();
            Entity player = commandBuffer.Instantiate(spawner.Prefab);

            Vector3 position = GameObjectLocator.Instance.Tilemap.CellToWorld(new(TilemapSystem.Center.x, TilemapSystem.Center.y));
            commandBuffer.SetComponent(player, new LocalTransform()
            {
                Position = position,
                Scale = 0.5f,
            });

            // Destroy spawner tag entity
            foreach (var (_, entity) in SystemAPI.Query<RefRW<PlayerSpawnerEnableData>>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(entity);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}