using Buildings;
using Buildings.Base;
using Pathfinding.Algorithm;
using Players;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileBaseLookup;

namespace Tilemaps
{
    public partial class TilemapSystem : SystemBase
    {
        public static int2 Center = new(20, 20);
        private bool updatePlayer;

        protected override void OnCreate()
        {
            this.RequireForUpdate<TilemapData>();
            this.RequireForUpdate<TilemapNodesData>();
            this.RequireForUpdate<BaseData>();
            this.RequireForUpdate<PlayerMovementData>();
            this.updatePlayer = true;
        }

        protected override void OnUpdate()
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
            Vector3 gridCenter = tilemap.GetCellCenterWorld(new(Center.x, Center.y, 0));
            tilemapData.ValueRW.CenterOfGrid = new(gridCenter.x, gridCenter.y);
            tilemapData.ValueRW.CenterCell = Center;

            // Update player
            if (this.updatePlayer)
            {
                this.updatePlayer = false;

                /*
                foreach (var (transform, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerMovementData>>())
                {
                    transform.ValueRW.Position = new(tilemapData.ValueRW.CenterOfGrid.x, tilemapData.ValueRW.CenterOfGrid.y, -5);
                }
                */

                foreach (var (transform, _, buildingData) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<BaseData>, RefRW<BuildingData>>())
                {
                    buildingData.ValueRW.Index = Center;
                    transform.ValueRW.Position = new(tilemapData.ValueRO.CenterOfGrid.x, tilemapData.ValueRO.CenterOfGrid.y, -5);
                    transform.ValueRW.Scale = 0.5f;
                }
            }
        }
    }
}