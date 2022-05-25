using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaData : MonoBehaviour
{
    public Tile[] MapData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Entity> Entities { get; protected set; } = new List<Entity>();
    public List<Feature> Features { get; protected set; } = new List<Feature>();

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

    public Tile[] GetNeighboringTiles(Tile t)
    {
        List<Tile> neighbors = new List<Tile>();
        foreach (Direction d in (Direction[])System.Enum.GetValues(typeof(Direction)))
        {
            neighbors.Add(GetNeighboringTile(d, t));
        }

        return neighbors.ToArray();
    }

    public static AreaData GetAreaDataForCurrentType()
    {
        if (CurrentMapType.Type == MapType.World)
        {
            return WorldData.Instance;
        }
        else
        {
            return LocationData.Instance;
        }
    }

    public static Tile[] GetMapDataForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            WorldData.Instance.MapData :
            LocationData.Instance.MapData;
    }

    public static List<Entity> GetEntitiesForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            WorldData.Instance.Entities :
            LocationData.Instance.Entities;
    }

    public static List<Feature> GetFeaturesForCurrentType()
    {
        return CurrentMapType.Type == MapType.World ?
            WorldData.Instance.Features :
            LocationData.Instance.Features;
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
}
