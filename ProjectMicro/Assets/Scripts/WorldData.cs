using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton holding world data
/// </summary>
public class WorldData : AreaData
{
    public static WorldData Instance { get; private set; }
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

    private void Start()
    {
        //SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
    }

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

    public Tile[] GetNeighboringTiles(Tile t)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Direction d in (Direction[])System.Enum.GetValues(typeof(Direction)))
        {
            neighbors.Add(GetNeighboringTile(d, t));
        }

        return neighbors.ToArray();
    }

    /// <summary>
    /// Sets each tile's list of neighbors. Used when create the tile array.
    /// </summary>
    public static void SetTileNeighbors()
    {
        for (int i = 0; i < LocationData.Instance.MapData.Length; i++)
        {
            Tile[] neighbors =
                LocationData.Instance.GetNeighboringTiles(
                    LocationData.Instance.MapData[i]);
            LocationData.Instance.MapData[i].SetNeighbors(neighbors);
        }
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

    private void OnDataLoaded(LoadedLocationData loadedLocationData)
    {
        ClearOldData();

        Width = loadedLocationData.Width;
        Height = loadedLocationData.Height;

        MapData = loadedLocationData.MapData;

        Entities = loadedLocationData.Entities;
        Features = loadedLocationData.Features;

        FindObjectOfType<LocationGenerator>().OnDataLoaded(Entities);
        GenerateTileGraph();
    }

    private void ClearOldData()
    {
        foreach (Entity entity in Entities)
        {
            entity.Destroy();
        }
        FindObjectOfType<AIController>().ClearAll();
        FindObjectOfType<SpriteDisplay>().ClearAll();
    }
}
