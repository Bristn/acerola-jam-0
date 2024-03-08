using System;
using Buildings.Base;
using Cameras.Targets;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static TilemapHelpers;
using static TowerHelpers;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TowerPlacemementSystem : SystemBase
    {
        public static Action FinishedPlacement;
        public static Action NotEnoughResources;
        public static Action PositionAlreadyOccupied;

        protected override void OnCreate()
        {
            this.RequireForUpdate<TowerPlacemementData>();
        }

        protected override void OnUpdate()
        {
            RefRW<TowerPlacemementData> placementData = SystemAPI.GetSingletonRW<TowerPlacemementData>();
            if (!placementData.ValueRO.ShowPlacement)
            {
                this.HideVisualiser();
                return;
            }

            TowerInformation information = TowerHelpers.Towers[placementData.ValueRO.TowerType];

            // Update the visualiser 
            CellData cellData = TilemapHelpers.GetCellDataAtScreen(Input.mousePosition);
            foreach (var (transform, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TowerPlacemementData>>())
            {
                transform.ValueRW.Position = cellData.Center;
                transform.ValueRW.Scale = information.Radius;
            }

            // Following code hanldes placing of tower. Therefore only execute once if the mouse is released
            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            placementData.ValueRW.ShowPlacement = false;
            FinishedPlacement.Invoke();

            // Check if enough money is present
            RefRW<BaseData> baseData = SystemAPI.GetSingletonRW<BaseData>();
            if (baseData.ValueRO.BuildingResoruces < information.Cost)
            {
                NotEnoughResources?.Invoke();
                return;
            }

            // Check if cell is already occupied
            foreach (var building in SystemAPI.Query<RefRO<BuildingData>>())
            {
                if (building.ValueRO.Index.Equals(cellData.Index))
                {
                    PositionAlreadyOccupied?.Invoke();
                    return;
                }
            }

            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            this.PlaceTower(commandBuffer, cellData, information);

            // Update player materials
            baseData.ValueRW.BuildingResoruces -= information.Cost;
            commandBuffer.Playback(this.EntityManager);
            commandBuffer.Dispose();
        }

        private void HideVisualiser()
        {
            foreach (var (transform, data) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TowerPlacemementData>>())
            {
                transform.ValueRW.Position.z = 100;
            }
        }

        private void PlaceTower(EntityCommandBuffer commandBuffer, CellData cellData, TowerInformation towerInformation)
        {
            // Place tower
            Entity prefab = SystemAPI.GetSingleton<TowerSpawnerData>().TowerPrefab;
            Entity instance = commandBuffer.Instantiate(prefab);

            // Add Components to new instance
            commandBuffer.AddComponent<BuildingData>(instance);
            commandBuffer.SetComponent(instance, new BuildingData()
            {
                Index = cellData.Index
            });

            // Set tower values
            commandBuffer.SetComponent(instance, new TowerData()
            {
                Radius = towerInformation.Radius,
                BulletVelocity = towerInformation.BulletVelocity,
                BulletCountPerShot = towerInformation.BulletCountPerShot,
                BulletRandomness = towerInformation.BulletRandomness,

                // Newly placed towers don't have to reload...
                TotalFireCooldown = towerInformation.FireCooldown,
                CurrentFireCooldown = 0,
                CanFire = false,

                // ... but they need to target their enemies first
                TotalTargettingTime = towerInformation.TargettingTime,
                CurrentTargettingTime = towerInformation.TargettingTime,
                HasTarget = false,
            });

            // Position prefab
            commandBuffer.SetComponent(instance, new LocalTransform()
            {
                Position = cellData.Center,
                Scale = 0.5f,
                Rotation = new(0, 0, 0, 1)
            });
        }
    }
}