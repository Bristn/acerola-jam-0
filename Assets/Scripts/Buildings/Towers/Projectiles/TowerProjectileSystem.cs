using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TowerProjectileSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerProjectileSystem: OnCreate");
            state.RequireForUpdate<TowerProjectileData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveProjectileJob()
            {
                DeltaTime = Time.deltaTime
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveProjectileJob : IJobEntity
        {
            public float DeltaTime;

            public void Execute(ref LocalTransform tranform, in TowerProjectileData projectile)
            {
                float2 moveBy = projectile.Direction * projectile.Speed * DeltaTime;
                tranform.Position += new float3(moveBy.x, moveBy.y, 0);

                // TODO: Check for hits
                // TODO: Check if projectile should be destroyed
            }
        }
    }
}