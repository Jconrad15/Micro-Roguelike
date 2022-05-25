using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private int seed = 0;
    [SerializeField]
    private int width = 30;
    [SerializeField]
    private int height = 30;

    private LocationGenerator locationGenerator;

    // TODO: make these be determine based on player location in the world
    private int worldX = 1;
    private int worldY = 1;

    public static WorldGenerator Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        locationGenerator = FindObjectOfType<LocationGenerator>();
        InitializeItemDatabase();
    }

    private void InitializeItemDatabase()
    {
        ItemDatabase.CreateDatabase();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            locationGenerator.StartGenerateLocation(
                seed, width, height, worldX, worldY);
        }
    }

    private void CreateMapData()
    {
        LocationData.Instance.MapData = new Tile[width * height];
        LocationData.Instance.Width = width;
        LocationData.Instance.Height = height;

        // Create base tile type map
        TileType[] rawMap = CreateRawMapData();

        // Set tile types
        for (int i = 0; i < LocationData.Instance.MapData.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            // Create walls
            // For now just generate walls here
            if ((x == 5 && y >= 5 && y <= 10) ||
                (y == 10 && x >= 0 && x <= 10) ||
                (y == 10 && x >= 12 && x <= 30))
            {
                LocationData.Instance.MapData[i] = new Tile(x, y, TileType.Wall);
                continue;
            }

            // Otherwise set to open tile
            LocationData.Instance.MapData[i] = new Tile(x, y, rawMap[i]);
        }

        LocationData.SetTileNeighbors();
        LocationData.Instance.GenerateTileGraph();
    }

    private TileType[] CreateRawMapData()
    {
        TileType[] rawMap = new TileType[width * height];

        SimplexNoise.Seed = seed;
        float scale = 0.1f;

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            float sample = SimplexNoise.CalcPixel2D(x, y, scale) / 255f;
            if (sample <= 0.2f)
            {
                rawMap[i] = TileType.Water;
            }
            else
            {
                rawMap[i] = TileType.OpenArea;
            }
        }

        return rawMap;
    }

}
