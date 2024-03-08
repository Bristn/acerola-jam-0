using System;
using Buildings.Base;
using Cameras.Targets;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TilemapHelpers;
using static TowerHelpers;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TowerPlacemementSystem : ISystem
    {
        public static Action NotEnoughResources;
        public static Action PositionAlreasyOccupied;
        public static Action<int> ResourcesUpdated;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerPlacememenSystem: OnCreate");
            state.RequireForUpdate<TowerPlacemementData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<TowerPlacemementData> placementData = SystemAPI.GetSingletonRW<TowerPlacemementData>();
            if (!placementData.ValueRO.ShowPlacement)
            {
                this.HideVisualiser(ref state);
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

            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            placementData.ValueRW.ShowPlacement = false;

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
                    PositionAlreasyOccupied?.Invoke();
                    return;
                }
            }

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
                Radius = information.Radius,
                TotalFireCooldown = information.FireCooldown,
                CurrentFireCooldown = 0,
                CanFire = false,
            });

            // Position prefab
            commandBuffer.SetComponent(instance, new LocalTransform()
            {
                Position = cellData.Center,
                Scale = 0.5f,
                Rotation = new(0, 0, 0, 1)
            });

            // Update player materials
            baseData.ValueRW.BuildingResoruces -= information.Cost;
            ResourcesUpdated?.Invoke(baseData.ValueRO.BuildingResoruces);
        }

        private void HideVisualiser(ref SystemState state)
        {
            foreach (var (transform, data) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TowerPlacemementData>>())
            {
                transform.ValueRW.Position.z = 100;
            }
        }
    }
}