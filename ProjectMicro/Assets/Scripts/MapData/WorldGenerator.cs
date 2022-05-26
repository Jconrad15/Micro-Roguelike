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

    private int playerWorldX;
    private int playerWorldY;

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

        FindObjectOfType<PlayerController>()
            .RegisterOnPlayerGoToExitTile(OnPlayerGoToExitTile);
    }

    private void Start()
    {
        StartGenerateWorld();
    }

    private void StartGenerateWorld()
    {
        playerWorldX = width / 2;
        playerWorldY = 0;

        CurrentMapType.SetCurrentMapType(MapType.World);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateWorldMapData();

        PlayerInstantiation.CreatePlayer(playerWorldX, playerWorldY);
        AIEntityInstantiation.CreateAIEntities(width, height);

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    private void ReturnToWorld(Player player)
    {
        CurrentMapType.SetCurrentMapType(MapType.World);

        // First need to destroy all current info
        DataLoader.ClearAllOldData();

        Random.State oldState = Random.state;
        Random.InitState(seed);

        //CreateWorldMapData();

        PlayerInstantiation.TransitionPlayerToMap(
            player, playerWorldX, playerWorldY);

        // TODO: generate entites in correct places, not randomly
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
        // Get player location
        Player player = FindObjectOfType<PlayerController>().GetPlayer();

        TileType tileType = player.T.type;

        // Save world location for now
        playerWorldX = player.X;
        playerWorldY = player.Y;

        // First need to destroy all current info
        DataLoader.ClearAllOldData();

        // Then load the location
        locationGenerator.StartGenerateLocation(
            seed, width, height,
            playerWorldX, playerWorldY,
            tileType, player);
    }

    private void CreateWorldMapData()
    {
        WorldData.Instance.MapData = new Tile[width * height];
        WorldData.Instance.Width = width;
        WorldData.Instance.Height = height;

        // Create tile types

        // Create base tile type map
        RawMapData rawMapData = new RawMapData(width, height, seed);

        // Set tile types
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            WorldData.Instance.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        WorldData.Instance.SetTileNeighbors();
        WorldData.Instance.GenerateTileGraph();

        // Create features

        // Place urban area tiles
        PlaceUrbanCenter(rawMapData);
    }

    private static void PlaceUrbanCenter(RawMapData rawMapData)
    {
        for (int i = 0; i < rawMapData.potentialCityLocations.Length; i++)
        {
            int index = rawMapData.potentialCityLocations[i];

            // Don't place cities in the water
            if (WorldData.Instance.MapData[index].type != TileType.Water)
            {
                // Choose between city and town
                if (Random.value > 0.5)
                {
                    WorldData.Instance.MapData[index].feature =
                    new Feature(
                        FeatureType.Town,
                        WorldData.Instance.MapData[index]);
                }
                else
                {
                    WorldData.Instance.MapData[index].feature =
                        new Feature(
                            FeatureType.City,
                            WorldData.Instance.MapData[index]);
                }
                Debug.Log("UrbanAreaPlaced");
            }
        }
    }

    public void OnDataLoaded()
    {
        cbOnWorldCreated?.Invoke();
    }

    private void OnPlayerGoToExitTile(Player player)
    {
        ReturnToWorld(player);
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
