using Buildings.Base;
using Buildings.Towers;
using Cameras;
using Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

namespace Common.Time
{
    public partial struct TimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TimeSystem: OnCreate");
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, pauseEntity) in SystemAPI.Query<RefRW<PauseTimeData>>().WithEntityAccess())
            {
                commandBuffer.DestroyEntity(pauseEntity);

                foreach (var (_, ResumeEntity) in SystemAPI.Query<RefRW<ResumeTimeData>>().WithEntityAccess())
                {
                    commandBuffer.DestroyEntity(ResumeEntity);
                }
            }
        }
    }
}