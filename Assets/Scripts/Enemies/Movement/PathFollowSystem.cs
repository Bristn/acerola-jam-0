using Cameras.Targets;
using Enemies;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct PathFollowSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("PathFollowSystem: OnCreate");
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (pathBuffer, transform, pathFollow) in SystemAPI.Query<DynamicBuffer<PathPosition>, RefRW<LocalTransform>, RefRW<PathFollowData>>())
        {
            if (pathFollow.ValueRO.Index == -1)
            {
                continue;
            }

            int2 pathPosition = pathBuffer[pathFollow.ValueRO.Index].Position;

            float3 targetPosition = new(pathPosition.x, pathPosition.y, -5);
            float3 direction = math.normalizesafe(targetPosition - transform.ValueRO.Position);
            float moveSpeed = 3;

            transform.ValueRW.Position += direction * moveSpeed * Time.deltaTime;

            if (math.distance(transform.ValueRO.Position, targetPosition) < 0.1f)
            {
                pathFollow.ValueRW.Index--;
            }
        }
    }
}