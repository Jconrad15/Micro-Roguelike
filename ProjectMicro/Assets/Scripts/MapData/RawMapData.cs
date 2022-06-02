using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RawMapData
{
    public TileType[] rawMap;
    public int[] potentialCityLocations;

    /// <summary>
    /// Constructor for world raw map data
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="seed"></param>
    public RawMapData(
        int width, int height, int seed)
    {
        (rawMap, potentialCityLocations) =
            CreateWorldRawMapData(width, height, seed);
    }

    private (TileType[] rawMap, int[] potentialCityLocations) 
        CreateWorldRawMapData(int width, int height, int seed)
    {
        TileType[] rawMap = new TileType[width * height];
        
        int maxCategoryTypes = Enum.GetNames(typeof(TileType)).Length;
        int seedCount = (int)(width * height * 0.01f);

        (int[] categories, int[] seedIndicies) =
            Voronoi.JumpFlood(
                width, height, seed, seedCount, maxCategoryTypes);

        // Determine tiletypes based on voronoi categories
        for (int i = 0; i < rawMap.Length; i++)
        {
            rawMap[i] = (TileType)categories[i];

            // Switch any walls and exit tiles to open area tiles
            if (rawMap[i] == TileType.Wall)
            {
                rawMap[i] = TileType.OpenArea;
                continue;
            }
        }

        return (rawMap, seedIndicies);
    }

    /// <summary>
    /// Constructor for location raw map data.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="seed"></param>
    /// <param name="locationTileType"></param>
    public RawMapData(
        int width, int height,
        int seed, TileType locationTileType,
        int worldX, int worldY)
    {
        rawMap = CreateLocationRawMapData(
            width, height, seed, locationTileType,
            worldX, worldY);
    }

    private TileType[] CreateLocationRawMapData(
        int width, int height,
        int seed, TileType locationTileType,
        int worldX, int worldY)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        TileType[] rawMap = new TileType[width * height];

        int maxCategoryTypes = Enum.GetNames(typeof(TileType)).Length;
        int seedCount = width / 2;

        (int[] categories, int[] seedIndicies) =
            Voronoi.JumpFlood(
                width, height, seed, seedCount, maxCategoryTypes);

        // Determine tiletypes based on voronoi categories
        // and location tile type
        for (int i = 0; i < rawMap.Length; i++)
        {
            rawMap[i] = (TileType)categories[i];

            // Switch any walls to open area tiles
            if (rawMap[i] == TileType.Wall)
            {
                rawMap[i] = TileType.OpenArea;
            }

            // Change raw map based on location type in the world
            rawMap[i] = EditTileTypeForLocation(
                locationTileType, rawMap[i]);
        }

        Random.state = oldState;
        return rawMap;
    }

    private static TileType EditTileTypeForLocation(
        TileType locationTileType, TileType rawMapTile)
    {
        // Edit voronoi map based on world location type

        // For water maps
        if (locationTileType == TileType.Water)
        {
            if (rawMapTile == TileType.Forest ||
                rawMapTile == TileType.Grass)
            {
                // For water maps, change forest and grass to water
                return TileType.Water;
            }
            else if (rawMapTile == TileType.OpenArea)
            {
                // Change percentage of openarea to water
                if (Random.value < 0.95f)
                {
                    return TileType.Water;
                }
            }
        }
        // For forest maps
        else if (locationTileType == TileType.Forest)
        {
            if (rawMapTile == TileType.Grass)
            {
                // Change percentage of grass to forest
                if (Random.value < 0.5f)
                {
                    return TileType.Forest;
                }
            }
            else if (rawMapTile == TileType.OpenArea)
            {
                // Change percentage of openarea to forest
                if (Random.value < 0.5f)
                {
                    return TileType.Forest;
                }
            }
            else if (rawMapTile == TileType.Water)
            {
                // Change percentage of openarea to water
                if (Random.value < 0.2f)
                {
                    return TileType.Forest;
                }
            }
        }
        // For openarea maps
        else if (locationTileType == TileType.OpenArea)
        {
            if (rawMapTile == TileType.Grass)
            {
                if (Random.value < 0.2f)
                {
                    return TileType.OpenArea;
                }
            }
            else if (rawMapTile == TileType.Forest)
            {
                if (Random.value < 0.8f)
                {
                    if (Random.value < 0.5f)
                    {
                        return TileType.OpenArea;
                    }
                    else
                    {
                        return TileType.Grass;
                    }
                }
            }
            else if (rawMapTile == TileType.Water)
            {
                if (Random.value < 0.5f)
                {
                    return TileType.OpenArea;
                }
            }
        }
        else if (locationTileType == TileType.Grass)
        {
            if (rawMapTile == TileType.OpenArea)
            {
                if (Random.value < 0.2f)
                {
                    return TileType.OpenArea;
                }
            }
            else if (rawMapTile == TileType.Forest)
            {
                if (Random.value < 0.8f)
                {
                    if (Random.value < 0.5f)
                    {
                        return TileType.OpenArea;
                    }
                    else
                    {
                        return TileType.Grass;
                    }
                }
            }
            else if (rawMapTile == TileType.Water)
            {
                if (Random.value < 0.5f)
                {
                    return TileType.Grass;
                }
            }
        }
        else
        {
            Debug.Log("Missing a world tile tile when " +
                "creating location raw map");
        }

        // Don't make an edit
        return rawMapTile;
    }
}
