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

}