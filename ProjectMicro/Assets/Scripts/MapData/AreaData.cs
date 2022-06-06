using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AreaData
{
    public bool IsWorld { get; private set; }

    public Tile[] MapData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Entity> Entities { get; protected set; } =
        new List<Entity>();
    public List<Feature> Features { get; protected set; } =
        new List<Feature>();

    public Path_TileGraph TileGraph { get; protected set; }

    public void GenerateTileGraph()
    {
        TileGraph = new Path_TileGraph();
    }

    public void AddEntity(Entity e)
    {
        Entities.Add(e);
    }

    public void AddFeature(Feature f)
    {
        Features.Add(f);
    }

    public void SetEntityList(List<Entity> entities)
    {
        Entities = entities;
    }

    public void SetFeatureList(List<Feature> features)
    {
        Features = features;
    }

    public Tile[] GetNeighboringTiles(Tile t)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Direction d in 
            (Direction[])System.Enum.GetValues(typeof(Direction)))
        {
            neighbors.Add(GetNeighboringTile(d, t));
        }

        return neighbors.ToArray();
    }

    public static AreaData GetAreaDataForCurrentType()
    {
        if (CurrentMapType.Type == MapType.World)
        {
            return AreaDataManager.Instance.WorldData;
        }
        else
        {
            return AreaDataManager.Instance.CurrentLocationData;
        }
    }

    public static Tile[] GetMapDataForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            AreaDataManager.Instance.WorldData.MapData :
            AreaDataManager.Instance.CurrentLocationData.MapData;
    }

    public static List<Entity> GetEntitiesForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            AreaDataManager.Instance.WorldData.Entities :
            AreaDataManager.Instance.CurrentLocationData.Entities;
    }

    public static List<Feature> GetFeaturesForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            AreaDataManager.Instance.WorldData.Features :
            AreaDataManager.Instance.CurrentLocationData.Features;
    }

    public Tile GetRandomWalkableTile()
    {
        Tile selectedTile = null;
        while (selectedTile == null)
        {
            int randomIndex = Random.Range(0, MapData.Length);
            if (MapData[randomIndex].isWalkable == true)
            {
                selectedTile = MapData[randomIndex];
            }
        }

        return selectedTile;
    }

    public Tile GetNeighboringTile(Direction d, Tile t)
    {
        int neighborX = t.x;
        int neighborY = t.y;

        if (d == Direction.N)
        {
            neighborY += 1;
        }
        else if (d == Direction.E)
        {
            neighborX += 1;
        }
        else if (d == Direction.S)
        {
            neighborY -= 1;
        }
        else // d == Direction.W
        {
            neighborX -= 1;
        }

        return GetTile(neighborX, neighborY);
    }

    public Tile GetTile(int i)
    {
        return MapData[i];
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0) { return null; }
        if (x >= Width || y >= Height) { return null; }

        return MapData[GetIndexFromCoord(x, y)];
    }

    public int GetIndexFromCoord(int x, int y)
    {
        return x + (y * Width);
    }

    public (int, int) GetCoordFromIndex(int i)
    {
        int x = i % Width;
        int y = i / Width % Height;
        return (x, y);
    }

    /// <summary>
    /// Sets each tile's list of neighbors. Used when create the tile array.
    /// </summary>
    public void SetTileNeighbors()
    {
        AreaData areaData = GetAreaDataForCurrentType();

        for (int i = 0; i < areaData.MapData.Length; i++)
        {
            Tile[] neighbors =
                areaData.GetNeighboringTiles(areaData.MapData[i]);
            areaData.MapData[i].SetNeighbors(neighbors);
        }
    }

    public void ClearAll()
    {
        MapData = null;
        Entities.Clear();
        Features.Clear();
        TileGraph = null;
    }
}
