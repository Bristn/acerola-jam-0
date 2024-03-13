using Buildings.Base;
using Pathfinding;
using Pathfinding.Followers;
using Pathfinding.Positions;
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
    public partial class EnemyMovementSystem : SystemBase
    {
        private Unity.Mathematics.Random random;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemyMovementSystem: OnCreate");
            RequireForUpdate<EnemyMovementData>();
            this.random = Unity.Mathematics.Random.CreateFromIndex(0);
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);


            foreach (var (enemyTransform, enemyMovement, follower, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<EnemyMovementData>, RefRW<PathFollowerData>>().WithEntityAccess())
            {
                float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);

                // Target the player if close enough
                float2 playerPosition = this.GetPlayerPosition();
                float distanceToPlayer = math.distance(enemyPosition, playerPosition);
                if (distanceToPlayer <= enemyMovement.ValueRO.PlayerDetectionRange)
                {
                    // Hit detection is handled in the EnemySystem
                    follower.ValueRW.CurrentCellIndex = -1;
                    follower.ValueRW.UseSlowSpeed = false;
                    this.MoveEnemyTowardsPosition(enemyPosition, playerPosition, enemyTransform);
                    continue;
                }

                // Move toward the base if in detection range
                float2 basePosition = this.GetBasePosition();
                float distanceToBase = math.distance(enemyPosition, basePosition);
                if (distanceToBase <= enemyMovement.ValueRO.BaseDetectionRange)
                {
                    this.SendPathfindingRequestForEnemy(commandBuffer, enemyPosition, entity, follower.ValueRO, TilemapSystem.Center);
                    follower.ValueRW.UseSlowSpeed = false;
                    continue;
                }

                // At this point, the enemy has to be outside the base detection range
                // Therefore move the enemy to a random neighboring cell
                // Only send a new pathfinding request if the enemy is not moving
                if (follower.ValueRO.CurrentCellIndex != -1)
                {
                    continue;
                }

                enemyMovement.ValueRW.ReducePassiveMoveDelay(SystemAPI.Time.DeltaTime);
                if (enemyMovement.ValueRO.AllowPassiveMovement)
                {
                    // Determine the new target
                    float2 minMovement = new(-enemyMovement.ValueRO.PassiveMoveRadius, -enemyMovement.ValueRO.PassiveMoveRadius);
                    float2 maxMovement = new(enemyMovement.ValueRO.PassiveMoveRadius, enemyMovement.ValueRO.PassiveMoveRadius);
                    float2 movement = this.random.NextFloat2(minMovement, maxMovement);
                    float2 targetPosition = enemyPosition + movement;

                    // Send a pathfinding request
                    Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
                    Vector3Int targetCell = tilemap.WorldToCell(new(targetPosition.x, targetPosition.y, 0));
                    TileType tileType = TileBaseLookup.Instance.GetTileType(targetCell.x, targetCell.y);
                    if (!TileBaseLookup.Instance.IsWalkable(tileType))
                    {
                        continue;
                    }

                    this.SendPathfindingRequestForEnemy(commandBuffer, enemyPosition, entity, follower.ValueRO, new(targetCell.x, targetCell.y));

                    // Asign a new wait time
                    enemyMovement.ValueRW.PassiveMovementDelay = this.random.NextFloat(5);
                    enemyMovement.ValueRW.AllowPassiveMovement = false;
                    follower.ValueRW.UseSlowSpeed = true;
                }
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        private void MoveEnemyTowardsPosition(float2 enemyPosition, float2 targetPosition, RefRW<LocalTransform> enemyTransform)
        {
            float2 direction = math.normalizesafe(targetPosition - enemyPosition);
            float3 directionVec = new(direction.x, direction.y, 0);
            enemyTransform.ValueRW.Position += directionVec * 2 * SystemAPI.Time.DeltaTime;
        }

        private void SendPathfindingRequestForEnemy(EntityCommandBuffer commandBuffer, float2 enemyPosition, Entity enemy, PathFollowerData follower, int2 targetCell)
        {
            // If the enemy is already targetting the cell, don't change anything
            if (follower.CurrentCellIndex != -1 && SystemAPI.HasBuffer<PathPosition>(enemy))
            {
                DynamicBuffer<PathPosition> positions = SystemAPI.GetBuffer<PathPosition>(enemy);
                if (positions.Length != 0 && positions[0].GridPosition.x == targetCell.x && positions[0].GridPosition.y == targetCell.y)
                {
                    return;
                }
            }

            // Update the pathfinding by setting a new request
            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
            Vector3Int originCell = tilemap.WorldToCell(new(enemyPosition.x, enemyPosition.y, 0));

            commandBuffer.AddComponent(enemy, new PathfindingRequestPathData()
            {
                EndCell = targetCell,
                StartCell = new(originCell.x, originCell.y),
            });
        }

        private float2 GetBasePosition()
        {
            foreach (var (baseTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<BaseData>>())
            {
                return new(baseTransform.ValueRO.Position.x, baseTransform.ValueRO.Position.y);
            }

            return new(float.MaxValue, float.MaxValue);
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