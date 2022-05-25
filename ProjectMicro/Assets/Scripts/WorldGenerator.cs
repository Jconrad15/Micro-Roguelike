using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private int seed = 0;
    [SerializeField]
    private int width = 30;
    [SerializeField]
    private int height = 30;

    private LocationGenerator locationGenerator;

    private Action cbOnWorldCreated;

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

    private void Start()
    {
        StartGenerateWorld();
    }

    private void StartGenerateWorld()
    {
        CurrentMapType.SetCurrentMapType(MapType.World);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateWorldMapData();
        //CreateWorldFeatures();
        PlayerInstantiation.CreatePlayer(width / 2, 0);
        AIEntityInstantiation.CreateAIEntities(width, height);

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    private void InitializeItemDatabase()
    {
        ItemDatabase.CreateDatabase();
    }

    void Update()
    {
        // If on world map
        if (CurrentMapType.Type == MapType.World)
        {
            // If hit space bar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EnterLocation();
            }
        }
    }

    private void EnterLocation()
    {
        // First need to destroy all current info
        WorldData.Instance.ClearAllOldData();


        // Then load the location
        locationGenerator.StartGenerateLocation(
            seed, width, height, worldX, worldY);
    }

    private void CreateWorldMapData()
    {
        WorldData.Instance.MapData = new Tile[width * height];
        WorldData.Instance.Width = width;
        WorldData.Instance.Height = height;

        // Create base tile type map
        TileType[] rawMap = CreateRawMapData();

        // Set tile types
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

/*            // Create walls
            // For now just generate walls here
            if ((x == 5 && y >= 5 && y <= 10) ||
                (y == 10 && x >= 0 && x <= 10) ||
                (y == 10 && x >= 12 && x <= 30))
            {
                WorldData.Instance.MapData[i] = new Tile(x, y, TileType.Wall);
                continue;
            }*/

            // Otherwise set to open tile
            WorldData.Instance.MapData[i] = new Tile(x, y, rawMap[i]);
        }

        WorldData.Instance.SetTileNeighbors();
        WorldData.Instance.GenerateTileGraph();
    }

    private TileType[] CreateRawMapData()
    {
        TileType[] rawMap = new TileType[width * height];

        SimplexNoise.Seed = seed;
        float scale = 0.03f;

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            float sample = SimplexNoise.CalcPixel2D(x, y, scale) / 255f;
            if (sample <= 0.3f)
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

    public void RegisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldCreated += callbackfunc;
    }

    public void UnregisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldCreated -= callbackfunc;
    }

}
