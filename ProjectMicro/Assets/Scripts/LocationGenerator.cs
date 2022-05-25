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
        int worldX, int worldY, TileType locationTileType)
    {
        CurrentMapType.SetCurrentMapType(MapType.Location);

        this.seed = seed + worldX + worldY;
        this.width = width;
        this.height = height;

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateMapData(locationTileType);
        CreateFeatures();
        PlayerInstantiation.CreatePlayer(width / 2, 0);
        AIEntityInstantiation.CreateAIEntities(width, height);

        Random.state = oldState;

        cbOnLocationCreated?.Invoke();
    }

    private void CreateMapData(TileType locationTileType)
    {
        LocationData.Instance.MapData = new Tile[width * height];
        LocationData.Instance.Width = width;
        LocationData.Instance.Height = height;

        // Create base tile type map
        RawMapData rawMapData =
            new RawMapData(width, height, seed,locationTileType);

        // Set tile types
        for (int i = 0; i < LocationData.Instance.MapData.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            // Create walls
            // For now just generate walls here
            if ((x == 5 && y >= 5 && y <= 10) ||
                (y == 10 && x >= 0 && x <= 10) ||
                (y == 10 && x >= 12 && x <= width))
            {
                LocationData.Instance.MapData[i] =
                    new Tile(x, y, TileType.Wall);
                continue;
            }

            // Otherwise set to open tile
            LocationData.Instance.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        LocationData.Instance.SetTileNeighbors();
        LocationData.Instance.GenerateTileGraph();
    }

    /// <summary>
    /// Creates the features in the location.
    /// </summary>
    private void CreateFeatures()
    {
        Tile[] mapdata = LocationData.Instance.MapData;
        for (int i = 0; i < mapdata.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            if (x == 11 && y == 10)
            {
                mapdata[i].feature =
                    new Feature(FeatureType.Door, mapdata[i]);
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
