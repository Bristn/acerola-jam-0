using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapHelpers
{
    public static CellData GetCellDataAtScreen(Vector3 screenPosition)
    {
        Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector3Int cellIndex = tilemap.WorldToCell(worldPosition);
        Vector3 center = tilemap.GetCellCenterWorld(cellIndex);

        return new CellData()
        {
            Index = new int2(cellIndex.x, cellIndex.y),
            Center = new float3(center.x, center.y, 0)
        };
    }

    public struct CellData
    {
        public int2 Index;
        public float3 Center;
    }
}