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

namespace Enemies
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class EnemyMovementSystem : SystemBase
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemyMovementSystem: OnCreate");
            RequireForUpdate<EnemyMovementData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);


            foreach (var (enemyTransform, enemyMovement, follower, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyMovementData>, RefRW<PathFollowerData>>().WithEntityAccess())
            {
                float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);

                // Target the player if close enough
                float2 playerPosition = this.GetPlayerPosition();
                float distanceToPlayer = math.distance(enemyPosition, playerPosition);
                if (distanceToPlayer <= enemyMovement.ValueRO.PlayerDetectionRange)
                {
                    // Hit detection is handled in the EnemySystem
                    follower.ValueRW.CurrentCellIndex = -1;
                    this.MoveEnemyTowardsPosition(enemyPosition, playerPosition, enemyTransform);
                    continue;
                }

                // Move toward the base if in detection range
                float2 basePosition = this.GetBasePosition();
                float distanceToBase = math.distance(enemyPosition, basePosition);
                if (distanceToBase <= enemyMovement.ValueRO.BaseDetectionRange)
                {
                    this.SendPathfindingRequestForEnemy(commandBuffer, enemyPosition, entity, follower.ValueRO, TilemapSystem.Center);
                    continue;
                }

                // At this point, the enemy has to be outside the base detection range
                // Therefore move the enemy to a random neighboring cell
                // TODO:
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