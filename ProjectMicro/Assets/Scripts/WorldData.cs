using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton holding world data
/// </summary>
public class WorldData : MonoBehaviour
{
    public Tile[] MapData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Entity> Entities { get; private set; } = new List<Entity>();
    public List<Feature> Features { get; private set; } = new List<Feature>();

    public Path_TileGraph tileGraph { get; private set; }

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

        SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
    }

    public void GenerateTileGraph()
    {
        tileGraph = new Path_TileGraph();
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

    private void OnDataLoaded(LoadedWorldData loadedWorldData)
    {
        ClearOldData();

        Width = loadedWorldData.Width;
        Height = loadedWorldData.Height;

        MapData = loadedWorldData.MapData;

        Entities = loadedWorldData.Entities;
        Features = loadedWorldData.Features;

        FindObjectOfType<WorldGenerator>().OnDataLoaded(Entities);
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
