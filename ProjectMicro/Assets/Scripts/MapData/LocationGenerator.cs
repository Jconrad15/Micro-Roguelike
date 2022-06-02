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
    private int seed;

    private Action cbOnLocationCreated;

    /// <summary>
    /// Initiate generation of this location.
    /// </summary>
    /// <param name="seed"></param>
    public void StartGenerateLocation(int seed,
        int worldX, int worldY, TileType locationTileType, Player player,
        Feature locationFeature)
    {
        CurrentMapType.SetCurrentMapType(MapType.Location);
        
        // Modify seed for this generation to relate to world pos
        seed = (seed + worldX + worldY) * 10;
        this.seed = seed;

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

        Tile[] mapData = LocationData.Instance.MapData;

        bool[] isIndoor = new bool[mapData.Length];
        for (int i = 0; i < buildingCount; i++)
        {
            bool isPlaced = false;
            while (isPlaced == false)
            {
                // Determine location
                int index = Random.Range(0, mapData.Length);

                // return if already wall
                if (mapData[index].Type == TileType.Wall) { return; }

                int sizeX = Random.Range(6, 10);
                int sizeY = Random.Range(6, 10);
                (int x, int y) =
                    LocationData.Instance.GetCoordFromIndex(index);

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
                                LocationData.Instance
                                .GetTile(xDirection, yDirection);

                            //Check null
                            if (localNeighborTile == null) { continue; }

                            //Check if at edge of map
                            if (localNeighborTile.x >= LocationData.Instance.Width - 2)
                            {
                                doorMaxX = x;
                                continue; 
                            }
                            if (localNeighborTile.x <= 2)
                            {
                                doorMinX = x;
                                continue;
                            }
                            if (localNeighborTile.y >= LocationData.Instance.Height - 2)
                            { 
                                doorMaxY = y;
                                continue; 
                            }
                            if (localNeighborTile.y <= 2)
                            {
                                doorMinY = y;
                                continue;
                            }

                            isIndoor[LocationData.Instance.GetIndexFromCoord(xDirection, yDirection)] = true;
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

                    // Clamp door position based on potentially previously determined min/max values
                    doorX = Mathf.Clamp(doorX, doorMinX, doorMaxX);
                    doorY = Mathf.Clamp(doorY, doorMinY, doorMaxY);

                    int doorIndex = LocationData.Instance.GetIndexFromCoord(doorX, doorY);
                    mapData[doorIndex].feature = new Feature(FeatureType.Door, mapData[doorIndex]);
                }
            }
        }

        // Apply isIndoor data to mapData
        for (int x = 0; x < LocationData.Instance.Width; x++)
        {
            for (int y = 0; y < LocationData.Instance.Height; y++)
            {
                int index = LocationData.Instance.GetIndexFromCoord(x, y);

                // If this was designated as a door, set to open area and continue
                if (mapData[index].feature != null)
                {
                    if (mapData[index].feature.type == FeatureType.Door)
                    {
                        mapData[index].Type = TileType.OpenArea;
                        continue;
                    }
                }

                // if is indoor
                if (isIndoor[index] == true)
                {
                    // Get isIndoor neighbors
                    List<bool> isIndoorNeighbor = new List<bool>();

                    int index1 = LocationData.Instance.GetIndexFromCoord(x, y + 1);
                    if (index1 >= 0 && index1 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index1]);
                    }
                    int index2 = LocationData.Instance.GetIndexFromCoord(x + 1, y);
                    if (index2 >= 0 && index2 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index2]);
                    }
                    int index3 = LocationData.Instance.GetIndexFromCoord(x, y - 1);
                    if (index3 >= 0 && index3 <= mapData.Length)
                    {
                        isIndoorNeighbor.Add(isIndoor[index3]);
                    }
                    int index4 = LocationData.Instance.GetIndexFromCoord(x - 1, y);
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

                                mapData[index].Type = TileType.Wall;
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
                            // if all neighbors are inside, then this should be openarea
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
        LocationData.Instance.MapData = new Tile[locationWidth * locationHeight];
        LocationData.Instance.Width = locationWidth;
        LocationData.Instance.Height = locationHeight;

        // Create raw base tile type map
        RawMapData rawMapData =
            new RawMapData(locationWidth, locationHeight, seed, locationTileType,
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
            if (x == locationWidth - 1 || y == locationHeight - 1)
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
