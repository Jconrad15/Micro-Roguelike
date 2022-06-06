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

        AreaDataManager.Instance.SetCurrentMapType(MapType.World);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        AreaDataManager.Instance.SetWorldData(CreateWorldMapData(seed));

        PlayerInstantiation.CreatePlayer(playerWorldX, playerWorldY);
        AIEntityInstantiation.CreateInitialWorldEntities(seed);

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    private void ReturnToWorld(Player player)
    {
        int seed = GameInitializer.Instance.Seed;

        // First need to destroy all current info
        DataLoader.ResetAllOldData();
        AreaDataManager.Instance.SetCurrentMapType(MapType.World);

        Random.State oldState = Random.state;
        Random.InitState(seed);

        PlayerInstantiation.TransitionPlayerToMap(
            player, playerWorldX, playerWorldY);

        // TODO: generate entites in correct places, not randomly
        AIEntityInstantiation.LoadPreviousWorldEntities();

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    public void SavePlayerWorldPosition(int x, int y)
    {
        playerWorldX = x;
        playerWorldY = y;
    }

    public (int, int) GetSavedPlayerWorldPosition()
    {
        return (playerWorldX, playerWorldY);
    }

    private AreaData CreateWorldMapData(int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        AreaData worldData = new AreaData
        {
            MapData = new Tile[worldWidth * worldHeight],
            Width = worldWidth,
            Height = worldHeight
        };

        // Create tile types

        // Create base raw map
        RawMapData rawMapData =
            new RawMapData(worldWidth, worldHeight, seed);

        // Set tile types from raw map
        for (int i = 0; i < worldData.MapData.Length; i++)
        {
            (int x, int y) = worldData.GetCoordFromIndex(i);

            worldData.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        // Edit tile type map
        for (int i = 0; i < worldData.MapData.Length; i++)
        {
            // 1% small chance to randomly mutate tile type to open
            if (Random.value < 0.01f)
            {
                worldData.MapData[i].Type = TileType.OpenArea;
            }
        }

        worldData.SetTileNeighbors();
        worldData.GenerateTileGraph();

        // Create features

        // Place urban area tiles
        PlaceUrbanCenter(worldData, rawMapData, seed);

        Random.state = oldState;
        return worldData;
    }

    private AreaData PlaceUrbanCenter(
        AreaData worldData, RawMapData rawMapData, int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        for (int i = 0; i < rawMapData.potentialCityLocations.Length; i++)
        {
            int index = rawMapData.potentialCityLocations[i];

            // Determine not to place urban area in each voronoi cell
            if (Random.value < 0.8f) { continue; }

            // Don't place cities in the water
            if (worldData.MapData[index].Type != TileType.Water)
            {
                // Choose between city and town
                if (Random.value > 0.5)
                {
                    Feature f = new Feature(
                        FeatureType.Town,
                        worldData.MapData[index]);

                    worldData.MapData[index].TileFeature = f;
                    
                    // Add feature to feature list
                    worldData.AddFeature(f);
                }
                else
                {
                    Feature f = new Feature(
                            FeatureType.City,
                            worldData.MapData[index]);

                    worldData.MapData[index].TileFeature = f;

                    // Add feature to feature list
                    worldData.AddFeature(f);
                }
            }
        }

        Random.state = oldState;
        return worldData;
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
