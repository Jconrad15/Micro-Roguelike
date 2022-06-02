using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedAreaData
{
    public Tile[] MapData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Entity> Entities { get; set; }
    public List<Feature> Features { get; set; }
    public MapType MapType { get; set; }
    public int Seed { get; set; }

    public LoadedAreaData(Tile[] mapData, int width, int height,
        List<Entity> entities, List<Feature> features,
        MapType mapType, int seed)
    {
        MapData = mapData;
        Width = width;
        Height = height;
        Entities = entities;
        Features = features;
        MapType = mapType;
        Seed = seed;
    }
}
