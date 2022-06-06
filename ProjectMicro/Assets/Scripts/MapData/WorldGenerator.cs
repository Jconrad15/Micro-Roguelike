using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    private int worldWidth;
    private int worldHeight;

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
        FindObjectOfType<PlayerController>()
            .RegisterOnPlayerGoToExitTile(OnPlayerGoToExitTile);
    }

    public void StartGeneration(int worldWidth, int worldHeight)
    {
        this.worldHeight = worldHeight;
        this.worldWidth = worldWidth;

        StartGenerateWorld();
    }

    private void StartGenerateWorld()
    {
        int seed = GameInitializer.Instance.Seed;

        playerWorldX = worldWidth / 2;
        playerWorldY = worldHeight / 2;

        CurrentMapType.SetCurrentMapType(MapType.World);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateWorldMapData(seed);

        PlayerInstantiation.CreatePlayer(playerWorldX, playerWorldY);
        AIEntityInstantiation.CreateInitialWorldEntities(seed);

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    private void ReturnToWorld(Player player)
    {
        int seed = GameInitializer.Instance.Seed;

        CurrentMapType.SetCurrentMapType(MapType.World);

        // First need to destroy all current info
        DataLoader.ClearAllOldData();

        Random.State oldState = Random.state;
        Random.InitState(seed);

        //CreateWorldMapData();

        PlayerInstantiation.TransitionPlayerToMap(
            player, playerWorldX, playerWorldY);

        // TODO: generate entites in correct places, not randomly
        AIEntityInstantiation.GetPreviousWorldEntities();

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    public void SavePlayerWorldPosition(int x, int y)
    {
        playerWorldX = x;
        playerWorldY = y;
    }

    private void CreateWorldMapData(int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        AreaDataManager.Instance.WorldData.MapData = new Tile[worldWidth * worldHeight];
        AreaDataManager.Instance.WorldData.Width = worldWidth;
        AreaDataManager.Instance.WorldData.Height = worldHeight;

        // Create tile types

        // Create base raw map
        RawMapData rawMapData =
            new RawMapData(worldWidth, worldHeight, seed);

        // Set tile types from raw map
        for (int i = 0; i < AreaDataManager.Instance.WorldData.MapData.Length; i++)
        {
            (int x, int y) = AreaDataManager.Instance.WorldData.GetCoordFromIndex(i);

            AreaDataManager.Instance.WorldData.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        // Edit tile type map
        for (int i = 0; i < AreaDataManager.Instance.WorldData.MapData.Length; i++)
        {
            // 1% small chance to randomly mutate tile type to open
            if (Random.value < 0.01f)
            {
                AreaDataManager.Instance.WorldData.MapData[i].Type =
                    TileType.OpenArea;
            }
        }

        AreaDataManager.Instance.WorldData.SetTileNeighbors();
        AreaDataManager.Instance.WorldData.GenerateTileGraph();

        // Create features

        // Place urban area tiles
        PlaceUrbanCenter(rawMapData, seed);

        Random.state = oldState;
    }

    private static void PlaceUrbanCenter(RawMapData rawMapData, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < rawMapData.potentialCityLocations.Length; i++)
        {
            int index = rawMapData.potentialCityLocations[i];

            // Determine not to place urban area in each voronoi cell
            if (Random.value < 0.8f) { continue; }

            // Don't place cities in the water
            if (AreaDataManager.Instance.WorldData.MapData[index].Type != TileType.Water)
            {
                // Choose between city and town
                if (Random.value > 0.5)
                {
                    AreaDataManager.Instance.WorldData.MapData[index].TileFeature =
                    new Feature(
                        FeatureType.Town,
                        AreaDataManager.Instance.WorldData.MapData[index]);
                }
                else
                {
                    AreaDataManager.Instance.WorldData.MapData[index].TileFeature =
                        new Feature(
                            FeatureType.City,
                            AreaDataManager.Instance.WorldData.MapData[index]);
                }
            }
        }

        Random.state = oldState;
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
