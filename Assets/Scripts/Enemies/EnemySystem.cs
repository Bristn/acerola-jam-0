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
            foreach (var (baseTransform, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BaseData>>())
            {
                float2 basePosition = new(baseTransform.ValueRW.Position.x, baseTransform.ValueRW.Position.y);
                foreach (var (enemyTransform, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<EnemyData>>())
                {
                    float2 enemyPosition = new(enemyTransform.ValueRW.Position.x, enemyTransform.ValueRW.Position.y);
                    float distance = math.distance(basePosition, enemyPosition);

                    if (distance <= 0.15f)
                    {
                        EnemyReachedBase.Invoke();
                    }
                }
            }
        }
    }
}