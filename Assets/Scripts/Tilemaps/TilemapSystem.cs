using System.Collections.Generic;
using Buildings.Base;
using Pathfinding.Algorithm;
using Players;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileBaseLookup;

namespace Tilemaps
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct TilemapSystem : ISystem
    {
        private bool updatePlayer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TilemapSystem: OnCreate");
            this.updatePlayer = true;
            state.RequireForUpdate<TilemapData>();
            state.RequireForUpdate<BaseData>();
            state.RequireForUpdate<PlayerMovementData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<TilemapData> tilemapData = SystemAPI.GetSingletonRW<TilemapData>();
            if (tilemapData.ValueRO.IsUpdated)
            {
                return;
            }

            var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer commandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            DynamicBuffer<TilemapNodesData> buffer = SystemAPI.GetSingletonBuffer<TilemapNodesData>();
            buffer.Clear();

            Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
            BoundsInt bounds = tilemap.cellBounds;

            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int x = 0; x < bounds.size.x; x++)
                {
                    TileType tileType = TileBaseLookup.Instance.GetTileType(x, y);

                    PathNode node = new()
                    {
                        X = x,
                        Y = y,
                        Index = PathHelpers.GetCellIndex(x, y, bounds.size.x),
                        GCost = int.MaxValue,
                        IsWalkable = tileType != TileType.WATER_A,
                        PreviousNodeIndex = -1
                    };

                    node.UpdateFCost();

                    Vector3 worldPosition = tilemap.CellToWorld(new(x, y, 0));
                    buffer.Add(new()
                    {
                        Node = node,
                        WorldPosition = new(worldPosition.x, worldPosition.y),
                    });
                }
            }

            tilemapData.ValueRW.GridSize = new(bounds.size.x, bounds.size.y);
            tilemapData.ValueRW.IsUpdated = true;

            // Update center of grid
            tilemapData.ValueRW.CenterOfGrid = buffer[buffer.Length / 2].WorldPosition;

            // Update player
            if (this.updatePlayer)
            {
                this.updatePlayer = false;
                foreach (var (transform, movement) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerMovementData>>())
                {
                    transform.ValueRW.Position = new(tilemapData.ValueRW.CenterOfGrid.x, tilemapData.ValueRW.CenterOfGrid.y, -5);
                }

                // TODO: Why does base prefab not have a LocalTransform by default?
                foreach (var (baseBuilding, baseEntity) in SystemAPI.Query<RefRO<BaseData>>().WithEntityAccess())
                {
                    commandBuffer.AddComponent<LocalTransform>(baseEntity);
                    commandBuffer.SetComponent(baseEntity, new LocalTransform()
                    {
                        Position = new(tilemapData.ValueRW.CenterOfGrid.x, tilemapData.ValueRW.CenterOfGrid.y, -5),
                        Scale = 0.5f
                    });
                }
            }
        }
    }
}