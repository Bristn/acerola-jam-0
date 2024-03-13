using System;
using Buildings.Base;
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
            this.RequireForUpdate<PickupCollectorData>();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach (var (collectorTransform, collector) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<PickupCollectorData>>())
            {
                float2 collectorPosition = new(collectorTransform.ValueRO.Position.x, collectorTransform.ValueRO.Position.y);
                this.PickUpLoot(collectorPosition, collector, commandBuffer);
                this.DropOffLoot(collectorPosition, collector);
            }

            commandBuffer.Playback(this.EntityManager);
            commandBuffer.Dispose();
        }

        private void PickUpLoot(float2 collectorPosition, RefRW<PickupCollectorData> collector, EntityCommandBuffer commandBuffer)
        {
            // If colelctor is full, don't attract pickups
            if (collector.ValueRO.StoredPickups >= collector.ValueRO.MaxPickups)
            {
                return;
            }

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

        private void DropOffLoot(float2 collectorPosition, RefRW<PickupCollectorData> collector)
        {
            // If colelctor has no loot, return
            if (collector.ValueRO.StoredPickups == 0)
            {
                return;
            }

            foreach (var (baseTransform, baseData, baseEntity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<BaseData>>().WithEntityAccess())
            {
                float2 basePosition = new(baseTransform.ValueRO.Position.x, baseTransform.ValueRO.Position.y);
                float distance = math.distance(collectorPosition, basePosition);
                if (distance > 2f)
                {
                    continue;
                }

                // Check if pickup can be dropped
                baseData.ValueRW.AmmoResoruces += collector.ValueRW.StoredPickups * 1;
                collector.ValueRW.StoredPickups = 0;
                PickedUpLoot?.Invoke(collector.ValueRO.StoredPickups, collector.ValueRO.MaxPickups);
                continue;
            }
        }
    }
}