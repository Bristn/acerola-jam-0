using System.Collections.Generic;
using NaughtyAttributes;
using Pathfinding.Algorithm;
using Tilemaps;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InvalidTiles : MonoBehaviour
{
    public static InvalidTiles Instance { get; private set; }

    /* --- References --- */

    [SerializeField][BoxGroup("References")] private Tilemap mainTilemap;
    [SerializeField][BoxGroup("References")] private Tilemap tilemap;
    [SerializeField][BoxGroup("References")] private TileBase tile;

    /* --- Values --- */

    private BoundsInt gridBounds;
    private bool allCellsValid = false;
    private HashSet<int2> validCells = new();

    /* --- Methods --- */

    private void Start()
    {
        Instance = this;
        this.SetValidAround(TilemapSystem.Center, 5);
    }

    public void ResetTiles()
    {
        this.tilemap.ClearAllTiles();
        this.allCellsValid = true;
    }

    public bool IsCellValid(int2 cell)
    {
        return this.allCellsValid || this.validCells.Contains(cell);
    }

    public void SetValidAround(int2 center, int radius)
    {
        this.ResetTiles();
        this.gridBounds = this.mainTilemap.cellBounds;

        this.allCellsValid = false;
        this.validCells.Clear();
        this.validCells = this.GetNeighboringCells(center, radius);
        this.validCells.Add(center);

        List<Vector3Int> index = new();
        List<TileBase> cell = new();
        for (int x = gridBounds.xMin; x < gridBounds.xMax; x++)
        {
            for (int y = gridBounds.yMin; y < gridBounds.yMax; y++)
            {
                if (this.validCells.Contains(new(x, y)))
                {
                    continue;
                }

                index.Add(new(x, y, 0));
                cell.Add(this.tile);
            }
        }

        this.tilemap.SetTiles(index.ToArray(), cell.ToArray());
    }

    public HashSet<int2> GetNeighboringCells(int2 origin, int radius)
    {
        HashSet<int2> result = new();
        if (radius == 0)
        {
            return result;
        }

        List<int2> direct = this.GetNeighboringCells(origin);
        foreach (int2 neighbor in direct)
        {
            result.Add(neighbor);
            HashSet<int2> indirect = this.GetNeighboringCells(neighbor, radius - 1);
            foreach (int2 secondNeighbor in indirect)
            {
                result.Add(secondNeighbor);
            }
        }

        return result;
    }

    public List<int2> GetNeighboringCells(int2 origin)
    {
        List<int2> result = new();
        NativeArray<int2> offsets = origin.y % 2 == 0 ? PathHelpers.EvenNeighborOffsets : PathHelpers.OddNeighborOffsets;
        foreach (int2 offset in offsets)
        {
            int2 neighbor = origin + offset;
            if (neighbor.x < gridBounds.xMin || neighbor.y < gridBounds.yMin || neighbor.x > gridBounds.xMax || neighbor.y > gridBounds.yMax)
            {
                continue;
            }

            result.Add(neighbor);
        }

        return result;
    }
}