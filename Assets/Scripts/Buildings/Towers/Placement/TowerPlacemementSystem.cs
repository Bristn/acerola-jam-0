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
        public static Action<bool> FinishedPlacement;

        private int towerCount;

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

            // Allow to cancel placement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.HideVisualiser();
                placementData.ValueRW.ShowPlacement = false;
                FinishedPlacement.Invoke(false);
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

            // Check if cell is valid
            if (!InvalidTiles.Instance.IsCellValid(cellData.Index))
            {
                FinishedPlacement.Invoke(false);
                return;
            }

            // Check if enough money is present
            RefRW<BaseData> baseData = SystemAPI.GetSingletonRW<BaseData>();
            if (baseData.ValueRO.BuildingResoruces < information.Cost)
            {
                FinishedPlacement.Invoke(false);
                return;
            }

            // Check if cell is already occupied
            foreach (var building in SystemAPI.Query<RefRO<BuildingData>>())
            {
                if (building.ValueRO.Index.Equals(cellData.Index))
                {
                    FinishedPlacement.Invoke(false);
                    return;
                }
            }

            EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            this.PlaceTower(commandBuffer, cellData, information);
            FinishedPlacement.Invoke(true);

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
            Entity towerPrefab = SystemAPI.GetSingleton<TowerSpawnerData>().TowerPrefab;
            Entity towerInstance = commandBuffer.Instantiate(towerPrefab);

            // Add Components to new instance
            commandBuffer.AddComponent<BuildingData>(towerInstance);
            commandBuffer.SetComponent(towerInstance, new BuildingData()
            {
                Index = cellData.Index
            });

            // Set tower values
            commandBuffer.SetComponent(towerInstance, new TowerData()
            {
                Index = this.towerCount,
                Radius = towerInformation.Radius,
                BulletVelocity = towerInformation.BulletVelocity,
                BulletCountPerShot = towerInformation.BulletCountPerShot,
                BulletRandomness = towerInformation.BulletRandomness,

                // Newly placed towers don't have to reload
                TotalFireCooldown = towerInformation.FireCooldown,
                CurrentFireCooldown = 0,
                CanFire = false,
            });

            // Position prefab
            commandBuffer.SetComponent(towerInstance, new LocalTransform()
            {
                Position = cellData.Center,
                Scale = 0.5f,
                Rotation = new(0, 0, 0, 1)
            });

            // Create visualiser
            Entity visualiserPrefab = SystemAPI.GetSingleton<TowerSpawnerData>().VisualiserPrefab;
            Entity visualiserInstance = commandBuffer.Instantiate(visualiserPrefab);

            commandBuffer.AddComponent<TowerTargetVisualiserData>(visualiserInstance);
            commandBuffer.SetComponent(visualiserInstance, new TowerTargetVisualiserData()
            {
                TowerIndex = this.towerCount,
            });

            commandBuffer.SetComponent(visualiserInstance, new LocalTransform()
            {
                Position = new(cellData.Center.x, cellData.Center.y, 100),
                Scale = 25f,
                Rotation = new(0, 0, 0, 1),
            });

            this.towerCount++;
        }
    }
}