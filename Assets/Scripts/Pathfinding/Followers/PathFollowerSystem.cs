using System.ComponentModel;
using Common;
using Pathfinding.Algorithm;
using Pathfinding.Positions;
using Tilemaps;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pathfinding.Followers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PathFollowerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("PathFollowerSystem: OnCreate");
            state.RequireForUpdate<PathFollowerData>();
            state.RequireForUpdate<TilemapNodesData>();
            state.RequireForUpdate<ResumeTimeData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Check if tilemap is initialized
            TilemapData tilemapData = SystemAPI.GetSingleton<TilemapData>();
            if (!tilemapData.IsUpdated)
            {
                return;
            }

            DynamicBuffer<TilemapNodesData> tileBuffer = SystemAPI.GetSingletonBuffer<TilemapNodesData>();
            new MoveFollowerJob()
            {
                Speed = 1f,
                DeltaTime = Time.deltaTime,
                TileBuffer = tileBuffer,
                GridSize = tilemapData.GridSize
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveFollowerJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public DynamicBuffer<TilemapNodesData> TileBuffer; // Disable savety as access is readonly
            public int2 GridSize;
            public float DeltaTime;
            public float Speed;

            public void Execute(ref LocalTransform transform, ref DynamicBuffer<PathPosition> buffer, ref PathFollowerData followerData)
            {
                if (followerData.CurrentCellIndex == -1)
                {
                    return;
                }

                int2 targetCell = buffer[followerData.CurrentCellIndex].GridPosition;
                int targetIndex = PathHelpers.GetCellIndex(targetCell.x, targetCell.y, this.GridSize.x);
                float2 targetWorldPosition = this.TileBuffer[targetIndex].WorldPosition;
                float3 targetPosition = new(targetWorldPosition.x, targetWorldPosition.y, -5);
                float3 direction = math.normalizesafe(targetPosition - transform.Position);

                transform.Position += direction * this.Speed * this.DeltaTime;
                if (math.distance(transform.Position, targetPosition) < 0.1f)
                {
                    followerData.CurrentCellIndex--;
                }
            }
        }
    }
}