using Buildings.Towers;
using Common;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileBaseLookup;
using static TilemapHelpers;

namespace Players
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PlayerMovementSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("PlayerMovementSystem: OnCreate");
            RequireForUpdate<PlayerMovementData>();
            RequireForUpdate<PlayerMovementEnableData>();
            RequireForUpdate<ResumeTimeData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;

            // Check if there already is a building at this index
            foreach (var (transform, _, playerEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerMovementData>>().WithEntityAccess())
            {
                // Determine speed mult based on current tile
                float2 oldPosition = new(transform.ValueRW.Position.x, transform.ValueRW.Position.y);
                Vector3Int oldCellIndex = tilemap.WorldToCell(new(oldPosition.x, oldPosition.y));
                TileType oldTileType = TileBaseLookup.Instance.GetTileType(oldCellIndex.x, oldCellIndex.y);
                float speedMult = TileBaseLookup.Instance.GetSpeedModifier(oldTileType);

                float2 normalMovement = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * SystemAPI.Time.DeltaTime * 2 * speedMult;

                // Determine new position
                float2 newPosition = new(transform.ValueRW.Position.x + normalMovement.x, transform.ValueRW.Position.y + normalMovement.y);
                Vector3Int newCellIndex = tilemap.WorldToCell(new(newPosition.x, newPosition.y));
                TileType newTileType = TileBaseLookup.Instance.GetTileType(newCellIndex.x, newCellIndex.y);

                if (TileBaseLookup.Instance.IsWalkable(newTileType))
                {
                    transform.ValueRW.Position.x = newPosition.x;
                    transform.ValueRW.Position.y = newPosition.y;
                }

                // If the player is near the base, disable hitting by projectiles
                bool isNearBase = InvalidTiles.Instance.IsCellValid(new(newCellIndex.x, newCellIndex.y));
                bool isHittable = SystemAPI.HasComponent<TowerProjectileTargetData>(playerEntity);
                if (isNearBase & isHittable)
                {
                    commandBuffer.RemoveComponent<TowerProjectileTargetData>(playerEntity);
                }
                else if (!isNearBase & !isHittable)
                {
                    commandBuffer.AddComponent<TowerProjectileTargetData>(playerEntity);
                }
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}