using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Players
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("PlayerMovementSystem: OnCreate");
            state.RequireForUpdate<PlayerMovementData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float2 direction = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime;

            // Check if there already is a building at this index
            foreach (var (transform, target) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerMovementData>>())
            {
                transform.ValueRW.Position.x += direction.x;
                transform.ValueRW.Position.y += direction.y;
            }
        }
    }
}