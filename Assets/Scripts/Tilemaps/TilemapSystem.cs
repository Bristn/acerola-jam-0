using System.Collections.Generic;
using Pathfinding.Algorithm;
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
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Debug.Log("TilemapSystem: OnCreate");
            state.RequireForUpdate<TilemapData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<TilemapData> tilemapData = SystemAPI.GetSingletonRW<TilemapData>();
            if (tilemapData.ValueRO.IsUpdated)
            {
                return;
            }

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
                    buffer.Add(new() { Node = node });
                }
            }

            tilemapData.ValueRW.GridSize = new(bounds.size.x, bounds.size.y);
            tilemapData.ValueRW.IsUpdated = true;
        }
    }
}