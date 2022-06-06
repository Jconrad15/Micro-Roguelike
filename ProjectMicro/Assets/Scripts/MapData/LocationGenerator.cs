using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocationGenerator : MonoBehaviour
{
    [SerializeField]
    private int locationWidth = 30;
    [SerializeField]
    private int locationHeight = 30;

    private Action cbOnLocationCreated;

    public static LocationGenerator Instance { get; private set; }
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

    public void GenerateOrLoadLocation(
        int worldX, int worldY, TileType locationTileType,
        Player player, Feature locationFeature)
    {
        // Check if location data already exists
        bool locationDataExists =
            AreaDataManager.Instance.TryGetLocationData(
                worldX, worldY, out AreaData locationData);

        if (locationDataExists)
        {
            LoadLocation(locationData, player);
        }
        else
        {
            // if not, generate
            StartGenerateLocation(worldX, worldY, locationTileType,
                player, locationFeature);
        }
    }

    /// <summary>
    /// Initiate generation of this location.
    /// </summary>
    /// <param name="seed"></param>
    private void StartGenerateLocation(
        int worldX, int worldY, TileType locationTileType, Player player,
        Feature locationFeature)
    {
        AreaDataManager.Instance.SetCurrentMapType(MapType.Location);
        
        int seed = GameInitializer.Instance.Seed;
        // Modify seed for this generation to relate to world pos
        seed = (seed + worldX + worldY) * 10;

        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateLocationMapData(locationTileType, worldX, worldY);
        CreateExitLocationFeatures();
        TryCreateUrban(seed, locationFeature);

        PlayerInstantiation.TransitionPlayerToMap(
            player, locationWidth / 2, locationHeight / 2);
        AIEntityInstantiation.CreateLocationAIEntities(seed);

        Random.state = oldState;

        cbOnLocationCreated?.Invoke();
    }

    /// <summary>
    /// Load previously generated data of this location.
    /// </summary>
    /// <param name="seed"></param>
    private void LoadLocation(AreaData locationData, Player player)
    {
        AreaDataManager.Instance.SetCurrentMapType(MapType.Location);
        AreaDataManager.Instance.CurrentLocationData = locationData;

        PlayerInstantiation.TransitionPlayerToMap(
            player, locationWidth / 2, locationHeight / 2);
        AIEntityInstantiation.LoadLocationAIEntities(locationData);

        cbOnLocationCreated?.Invoke();
    }

    private void TryCreateUrban(int seed, Feature tileFeature)
    {
        if (tileFeature == null) { return; }

        // Create urban if city or town feature present in world tile
        if (tileFeature.type != FeatureType.City)
        {
            if (tileFeature.type != FeatureType.Town)
            {
                return;
            }
        }

        Random.State oldState = Random.state;
        Random.InitState(seed);

        int buildingCount = tileFeature.type == FeatureType.City ?
            Random.Range(10, 14) :
            Random.Range(5, 8);

        Tile[] mapData = AreaDataManager.Instance.CurrentLocationData.MapData;

        bool[] isIndoor = new bool[mapData.Length];
        for (int i = 0; i < buildingCount; i++)
        {
            bool isPlaced = false;
            while (isPlaced == false)
            {
                // Determine location
                int index = Random.Range(0, mapData.Length);

                // return if already a feature
                if (mapData[index].TileFeature != null) { continue; }

                int sizeX = Random.Range(6, 10);
                int sizeY = Random.Range(6, 10);
                (int x, int y) =
                    AreaDataManager.Instance.CurrentLocationData.GetCoordFromIndex(index);

                // Try placing isIndoor
                if (isIndoor[index] == false)
                {
                    isIndoor[index] = true;
                    isPlaced = true;
                    int doorMaxX = locationWidth - 1;
                    int doorMaxY = locationHeight - 1;
                    int doorMinX = 1;
                    int doorMinY = 1;
                    // Also set neighbors up to size x,y 
                    for (int xDirection = x - (sizeX / 2);
                        xDirection < x + (sizeX / 2); xDirection++)
                    {
                        for (int yDirection = y - (sizeY / 2);
                            yDirection < y + (sizeY / 2); yDirection++)
                        {
                            Tile localNeighborTile =
                                AreaDataManager.Instance.CurrentLocationData
                                .GetTile(xDirection, yDirection);

                            //Check null
                            if (localNeighborTile == null) { continue; }

                            //Check if at edge of map
                            if (localNeighborTile.x >=
                                AreaDataManager.Instance.CurrentLocationData.Width - 2)
                            {
                                doorMaxX = x;
                                continue; 
                            }
                            if (localNeighborTile.x <= 2)
                            {
                                doorMinX = x;
                                continue;
                            }
                            if (localNeighborTile.y >=
                                AreaDataManager.Instance.CurrentLocationData.Height - 2)
                            { 
                                doorMaxY = y;
                                continue; 
                            }
                            if (localNeighborTile.y <= 2)
                            {
                                doorMinY = y;
                                continue;
                            }

                            isIndoor[AreaDataManager.Instance.CurrentLocationData.GetIndexFromCoord(xDirection, yDirection)] = true;
                        }
                    }

                    int doorX, doorY;
                    // Set door
                    if (Random.value < 0.5f)
                    {
                        // Set door x
                        if (Random.value < 0.5f)
                        {
                            doorX = x + (sizeX / 2) - 1;
                            doorY = y;
                        }
                        else
                        {
                            doorX = x - (sizeX / 2);
                            doorY = y;
                        }
                    }
                    else
                    {
                        // Set door y
                        if (Random.value < 0.5f)
                        {
                            doorX = x;
                            doorY = y + (sizeY / 2) - 1;
                        }
                        else
                        {
                            doorX = x;
                            doorY = y - (sizeY / 2);
                        }
                    }

                    // Clamp door position based on potentially
                    // previously determined min/max values
                    doorX = Mathf.Clamp(doorX, doorMinX, doorMaxX);
                    doorY = Mathf.Clamp(doorY, doorMinY, doorMaxY);

                    int doorIndex = AreaDataManager.Instance.CurrentLocationData.GetIndexFromCoord(doorX, doorY);
                    mapData[doorIndex].TileFeature =
                        new Feature(FeatureType.Door, mapData[doorIndex]);
                    // Change door to open area
                    mapData[doorIndex].Type = TileType.OpenArea;
                }
            }
        }

        // Apply isIndoor data to mapData
        for (int x = 0; x < AreaDataManager.Instance.CurrentLocationData.Width; x++)
        {
            for (int y = 0; y < AreaDataManager.Instance.CurrentLocationData.Height; y++)
            {
                int index = AreaDataManager.Instance.CurrentLocationData.GetIndexFromCoord(x, y);

                // If this was designated as a feature, then just continue
                if (mapData[index].TileFeature != null)
                {
                    continue;
                }

                // if is indoor
                if (isIndoor[index] == true)
                {
                    // Get isIndoor neighbors
                    List<bool> isIndoorNeighbor = new List<bool>();

                    int index1 = AreaDataManager.Instance.CurrentLocationData
                        .GetIndexFromCoord(x, y + 1);
                    if (index1 >= 0 && index1 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index1]);
                    }
                    int index2 = AreaDataManager.Instance.CurrentLocationData
                        .GetIndexFromCoord(x + 1, y);
                    if (index2 >= 0 && index2 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index2]);
                    }
                    int index3 = AreaDataManager.Instance.CurrentLocationData
                        .GetIndexFromCoord(x, y - 1);
                    if (index3 >= 0 && index3 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index3]);
                    }
                    int index4 = AreaDataManager.Instance.CurrentLocationData
                        .GetIndexFromCoord(x - 1, y);
                    if (index4 >= 0 && index4 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index4]);
                    }

                    // Check if any neighbors are outdoor
                    foreach (bool n in isIndoorNeighbor)
                    {
                        // if neighbor is outdoor, then this should be a wall
                        if (n == false)
                        {
                            // Place wall here, but only 90% of the time
                            if (Random.value < 0.9f)
                            {
                                mapData[index].TileFeature = new Feature(
                                    FeatureType.Wall, mapData[index]);
                                // Also set tile to open area
                                mapData[index].Type = TileType.OpenArea;
                                break;
                            }
                            else
                            {
                                mapData[index].Type = TileType.OpenArea;
                                break;
                            }
                        }
                        else
                        {
                            // if all neighbors are inside,
                            // then this should be openarea
                            mapData[index].Type = TileType.OpenArea;
                        }
                    }
                }
            }
        }

        Random.state = oldState;
    }

    private void CreateLocationMapData(
        TileType locationTileType, int worldX, int worldY)
    {
        AreaDataManager.Instance.CurrentLocationData.MapData =
            new Tile[locationWidth * locationHeight];
        AreaDataManager.Instance.CurrentLocationData.Width = locationWidth;
        AreaDataManager.Instance.CurrentLocationData.Height = locationHeight;

        int seed = GameInitializer.Instance.Seed;

        // Create raw base tile type map
        RawMapData rawMapData = new RawMapData(
            locationWidth, locationHeight, seed,
            locationTileType, worldX, worldY);

        // Perform any edits to the raw base tile type map

        // Edit any tile types
        for (int i = 0; i < AreaDataManager.Instance.CurrentLocationData.MapData.Length; i++)
        {
            (int x, int y) = AreaDataManager.Instance.CurrentLocationData.GetCoordFromIndex(i);

            // Otherwise set to raw map tile
            AreaDataManager.Instance.CurrentLocationData.MapData[i] =
                new Tile(x, y, rawMapData.rawMap[i]);
        }

        AreaDataManager.Instance.CurrentLocationData.SetTileNeighbors();
        AreaDataManager.Instance.CurrentLocationData.GenerateTileGraph();
    }

    /// <summary>
    /// Creates the exit features in the location.
    /// </summary>
    private void CreateExitLocationFeatures()
    {
        Tile[] mapdata = AreaDataManager.Instance.CurrentLocationData.MapData;
        for (int i = 0; i < mapdata.Length; i++)
        {
            (int x, int y) = AreaDataManager.Instance.CurrentLocationData.GetCoordFromIndex(i);

            // Set edges to exit area
            if (x == 0 || y == 0)
            {
                mapdata[i].TileFeature =
                    new Feature(FeatureType.ExitLocation, mapdata[i]);
                continue;
            }
            if (x == locationWidth - 1 || y == locationHeight - 1)
            {
                mapdata[i].TileFeature =
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
