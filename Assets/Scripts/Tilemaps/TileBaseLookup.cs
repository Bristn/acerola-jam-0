using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using UnityEngine.Tilemaps;

public class TileBaseLookup : MonoBehaviour
{
    public static TileBaseLookup Instance { get; private set; }

    public enum TileType
    {
        NONE,
        WATER_A,
        WATER_B,
        SAND,
        GRASS_A,
        GRASS_B,
        ROCK_A,
        ROCK_B,
        SNOW
    }

    private void Start()
    {
        Instance = this;
    }

    [SerializedDictionary("Tile", "Type")]
    [SerializeField] private SerializedDictionary<TileBase, TileType> tileMapping = new();

    public TileType GetTileType(int x, int y)
    {
        Tilemap tilemap = GameObjectLocator.Instance.Tilemap;
        TileBase tileBase = tilemap.GetTile(new(x, y));
        if (tileBase == null)
        {
            return TileType.NONE;
        }

        if (this.tileMapping.TryGetValue(tileBase, out TileType type))
        {
            return type;
        }

        return TileType.NONE;
    }

    public bool IsWalkable(TileType type)
    {
        switch (type)
        {
            case TileType.NONE:
            case TileType.WATER_A:
            case TileType.SNOW:
                return false;

            case TileType.SAND:
            case TileType.WATER_B:
            case TileType.GRASS_A:
            case TileType.GRASS_B:
            case TileType.ROCK_A:
            case TileType.ROCK_B:
                return true;

            default:
                return false;
        }
    }

    public float GetSpeedModifier(TileType type)
    {
        switch (type)
        {
            case TileType.GRASS_A:
            case TileType.GRASS_B:
                return 1;

            case TileType.SAND:
            case TileType.ROCK_A:
                return 0.825f;

            case TileType.WATER_B:
            case TileType.ROCK_B:
                return 0.75f;

            default:
                return 1;
        }
    }

}