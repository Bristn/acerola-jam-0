using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Pickups
{
    public partial class PickupSystem : SystemBase
    {
        public static Action<int, int> PickedUpLoot;

        [BurstCompile]
        protected override void OnCreate()
        {
            this.RequireForUpdate<PickupData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach (var (collectorTransform, collector) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PickupCollectorData>>())
            {
                // If colelctor is full, don't attract pickups
                if (collector.ValueRO.StoredPickups >= collector.ValueRO.MaxPickups)
                {
                    continue;
                }

                float2 collectorPosition = new(collectorTransform.ValueRO.Position.x, collectorTransform.ValueRO.Position.y);
                foreach (var (pickupTransform, _, pickupEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PickupData>>().WithEntityAccess())
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
                        PickedUpLoot.Invoke(collector.ValueRO.StoredPickups, collector.ValueRO.MaxPickups);
                        continue;
                    }

                    // Move Pickup towards collector
                    float2 direction = math.normalize(collectorPosition - pickupPosition);
                    float speed = (collector.ValueRO.PickupRadius - distance) * 2f;
                    float2 moveBy = direction * speed * SystemAPI.Time.DeltaTime;
                    pickupTransform.ValueRW.Position += new float3(moveBy.x, moveBy.y, 0);
                }
            }

            commandBuffer.Playback(this.EntityManager);
            commandBuffer.Dispose();
        }
    }
}