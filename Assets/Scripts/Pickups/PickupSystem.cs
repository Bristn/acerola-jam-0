using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pickups
{
    public partial struct PickupSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("PickupSystem: OnCreate");
            state.RequireForUpdate<PickupData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (collectorTransform, collector) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PickupCollectorData>>())
            {
                float2 collectorPosition = new(collectorTransform.ValueRO.Position.x, collectorTransform.ValueRO.Position.y);

                foreach (var (pickupTransform, pickup, pickupEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PickupData>>().WithEntityAccess())
                {
                    float2 pickupPosition = new(pickupTransform.ValueRO.Position.x, pickupTransform.ValueRO.Position.y);
                    float distance = math.distance(collectorPosition, pickupPosition);
                    if (distance > collector.ValueRO.PickupRadius)
                    {
                        continue;
                    }

                    // Check if pickup can be collected
                    if (distance <= 0.1f)
                    {
                        commandBuffer.DestroyEntity(pickupEntity);
                        collector.ValueRW.StoredPickups++;
                        continue;
                    }

                    // Move Pickup towards collector
                    float2 direction = math.normalize(collectorPosition - pickupPosition);
                    float speed = collector.ValueRO.PickupRadius - distance;
                    float2 moveBy = direction * speed * Time.deltaTime;
                    pickupTransform.ValueRW.Position += new float3(moveBy.x, moveBy.y, 0);
                }
            }
        }
    }
}