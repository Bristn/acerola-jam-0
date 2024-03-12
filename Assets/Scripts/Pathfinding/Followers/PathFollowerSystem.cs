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
                Speed = 2f,
                DeltaTime = Time.deltaTime,
                TileBuffer = tileBuffer,
                GridSize = tilemapData.GridSize,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveFollowerJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public DynamicBuffer<TilemapNodesData> TileBuffer; // Disable savety as access is readonly
            public int2 GridSize;
            public float DeltaTime;
            public float Speed;
            public Unity.Mathematics.Random random;

            public void Execute(Entity entity, ref LocalTransform transform, ref DynamicBuffer<PathPosition> buffer, ref PathFollowerData follower)
            {
                if (follower.CurrentCellIndex == -1)
                {
                    return;
                }

                this.random = Unity.Mathematics.Random.CreateFromIndex((uint)(entity.Index + follower.CurrentCellIndex));

                int2 targetCell = buffer[follower.CurrentCellIndex].GridPosition;
                int targetIndex = PathHelpers.GetCellIndex(targetCell.x, targetCell.y, this.GridSize.x);
                float2 targetWorldPosition = this.TileBuffer[targetIndex].WorldPosition + follower.OffsetFromPath;
                float3 targetPosition = new(targetWorldPosition.x, targetWorldPosition.y, -5);
                float3 direction = math.normalizesafe(targetPosition - transform.Position);

                // Move to position
                transform.Position += direction * this.Speed * this.DeltaTime;

                // If close enough, change to next target
                if (math.distance(transform.Position, targetPosition) < 0.1f)
                {
                    follower.CurrentCellIndex--;
                    if (follower.CurrentCellIndex == 0)
                    {
                        follower.OffsetFromPath = new(0, 0);
                        return;
                    }

                    float width = 0.8659766f;
                    float2 min = new(-width / 2, -width / 2);
                    float2 max = new(width / 2, width / 2);
                    follower.OffsetFromPath = this.random.NextFloat2(min, max);
                }
            }
        }
    }
}