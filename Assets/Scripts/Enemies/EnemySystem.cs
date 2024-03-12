using System;
using Buildings.Base;
using Common;
using Common.Health;
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
    public partial class EnemySystem : SystemBase
    {
        public static Action EnemyReachedBase;

        [BurstCompile]
        protected override void OnCreate()
        {
            Debug.Log("EnemySystem: OnCreate");
            RequireForUpdate<EnemyData>();
            RequireForUpdate<ResumeTimeData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            bool hasReachedBase = this.CheckForReachedEnd();
            if (!hasReachedBase)
            {
                this.CheckForPlayerHit();
            }
        }

        private bool CheckForReachedEnd()
        {
            foreach (var (baseTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<BaseData>>())
            {
                float2 basePosition = new(baseTransform.ValueRO.Position.x, baseTransform.ValueRO.Position.y);
                foreach (var (enemyTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<EnemyData>>())
                {
                    float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);
                    float distance = math.distance(basePosition, enemyPosition);

                    if (distance <= 0.15f)
                    {
                        EnemyReachedBase.Invoke();
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckForPlayerHit()
        {
            foreach (var (playerTransform, _, health) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerMovementData>, RefRW<HealthData>>())
            {
                float2 playerPosition = new(playerTransform.ValueRO.Position.x, playerTransform.ValueRO.Position.y);

                foreach (var (enemyTransform, _) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<EnemyData>>())
                {
                    float2 enemyPosition = new(enemyTransform.ValueRO.Position.x, enemyTransform.ValueRO.Position.y);
                    float distance = math.distance(playerPosition, enemyPosition);

                    if (distance <= 0.15f)
                    {
                        health.ValueRW.CurrentHealth--;
                    }
                }
            }
        }
    }
}