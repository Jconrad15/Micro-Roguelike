using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocationGenerator : MonoBehaviour
{
    private int width;
    private int height;
    private int seed;

    private Action cbOnLocationCreated;

    /// <summary>
    /// Initiate generation of this location.
    /// </summary>
    /// <param name="seed"></param>
    public void StartGenerateLocation(int seed, int width, int height,
        int worldX, int worldY, TileType locationTileType, Player player,
        Feature locationFeature)
    {
        CurrentMapType.SetCurrentMapType(MapType.Location);

        this.seed = seed;
        this.width = width;
        this.height = height;

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateLocationMapData(locationTileType, worldX, worldY);
        CreateExitLocationFeatures();
        TryCreateUrban(locationFeature);

        PlayerInstantiation.TransitionPlayerToMap(
            player, width / 2, height / 2);
        AIEntityInstantiation.CreateAIEntities(seed);

        Random.state = oldState;

        cbOnLocationCreated?.Invoke();
    }

    private void TryCreateUrban(Feature tileFeature)
    {
        // Create urban if city or town feature present in world tile
        if (tileFeature.type != FeatureType.City ||
            tileFeature.type != FeatureType.Town)
        {
            return;
        }

        Tile[] mapData = LocationData.Instance.MapData;

        int buildingCount = tileFeature.type == FeatureType.City ?
            Random.Range(10, 20) :
            Random.Range(5, 10);

        for (int i = 0; i < buildingCount; i++)
        {
            TryCreateBuilding();
        }

    }

    // TODO: finish creating buildings
    private void TryCreateBuilding()
    {
        Tile[] mapData = LocationData.Instance.MapData;

        bool isPlaced = false;
        while (isPlaced == false)
        {
            // Determine location
            int index = Random.Range(0, mapData.Length);

            // return if already wall
            if (mapData[index].type == TileType.Wall) { return; }

            int sizeX = Random.Range(4, 10);
            int sizeY = Random.Range(4, 10);
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(index);


        }
    }

    private void CreateLocationMapData(
        TileType locationTileType, int worldX, int worldY)
    {
        LocationData.Instance.MapData = new Tile[width * height];
        LocationData.Instance.Width = width;
        LocationData.Instance.Height = height;

        // Create raw base tile type map
        RawMapData rawMapData =
            new RawMapData(width, height, seed, locationTileType,
            worldX, worldY);

        // Perform any edits to the raw base tile type map

        // Edit any tile types
        for (int i = 0; i < LocationData.Instance.MapData.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            // Otherwise set to raw map tile
            LocationData.Instance.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        LocationData.Instance.SetTileNeighbors();
        LocationData.Instance.GenerateTileGraph();
    }

    /// <summary>
    /// Creates the exit features in the location.
    /// </summary>
    private void CreateExitLocationFeatures()
    {
        Tile[] mapdata = LocationData.Instance.MapData;
        for (int i = 0; i < mapdata.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            // Set edges to exit area
            if (x == 0 || y == 0)
            {
                mapdata[i].feature =
                    new Feature(FeatureType.ExitLocation, mapdata[i]);
                continue;
            }
            if (x == width - 1 || y == height - 1)
            {
                mapdata[i].feature =
                    new Feature(FeatureType.ExitLocation, mapdata[i]);
                continue;
            }
        }
    }

    public void OnDataLoaded()
    {
        cbOnLocationCreated?.Invoke();
    }

    public void RegisterOnLocationCreated(Action callbackfunc)
    {
        cbOnLocationCreated += callbackfunc;
    }

    public void UnregisterOnLocationCreated(Action callbackfunc)
    {
        cbOnLocationCreated -= callbackfunc;
    }
}
