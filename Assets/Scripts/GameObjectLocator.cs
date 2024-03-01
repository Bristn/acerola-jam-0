using UnityEngine;
using UnityEngine.Tilemaps;

public class GameObjectLocator : MonoBehaviour
{
    public static GameObjectLocator Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }

    /* --- Tilemap --- */

    public Grid Grid;
    public Tilemap Tilemap;

    /* --- Player --- */

    public Camera MainCamera;
}