using Cameras.Targets;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Buildings.Towers
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TowerSpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TowerSpawnerSystem: OnCreate");
            state.RequireForUpdate<TowerSpawnerData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            if (GameObjectLocator.Instance.Tilemap == null)
            {
                return;
            }

            // Get world position of cell beneath mouse
            CellData cellData = this.GetCellData(Input.mousePosition);

            // Check if there already is a building at this index
            EntityManager entityManager = state.EntityManager;
            foreach (var building in SystemAPI.Query<RefRO<BuildingData>>())
            {
                if (building.ValueRO.Index.Equals(cellData.Index))
                {
                    Debug.Log("TowerSpawnerSystem: Cannot place tower as there already is a building at: " + cellData.Index.ToString());
                    return;
                }
            }

            // Instantiate prefab
            Entity prefab = SystemAPI.GetSingleton<TowerSpawnerData>().Prefab;
            Entity instance = entityManager.Instantiate(prefab);

            // Add Components to new instance
            entityManager.AddComponent<BuildingData>(instance);
            entityManager.SetComponentData(instance, new BuildingData()
            {
                Index = cellData.Index
            });

            // Position prefab
            RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(instance);
            transform.ValueRW.Position = cellData.Center;
        }

        private CellData GetCellData(Vector3 screenPosition)
        {
            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector3Int cellIndex = tilemap.WorldToCell(worldPosition);
            Vector3 center = tilemap.GetCellCenterWorld(cellIndex);

            Debug.Log(cellIndex);

            return new CellData()
            {
                Index = new int3(cellIndex.x, cellIndex.y, 0),
                Center = new float3(center.x, center.y, 0)
            };
        }

        private struct CellData
        {
            public int3 Index;
            public float3 Center;
        }
    }
}