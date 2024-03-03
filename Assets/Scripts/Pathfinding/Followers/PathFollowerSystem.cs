using Pathfinding.Positions;
using Unity.Burst;
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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveFollowerJob() { Speed = 3f, DeltaTime = Time.deltaTime }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveFollowerJob : IJobEntity
        {
            public float DeltaTime;
            public float Speed;

            public void Execute(ref LocalTransform transform, ref DynamicBuffer<PathPosition> buffer, ref PathFollowerData followerData)
            {
                if (followerData.CurrentCellIndex == -1)
                {
                    return;
                }

                int2 targetCell = buffer[followerData.CurrentCellIndex].GridPosition;
                float3 targetPosition = new(targetCell.x, targetCell.y, -5);
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