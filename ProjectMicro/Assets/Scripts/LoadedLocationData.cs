using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedLocationData
{
    public Tile[] MapData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<Entity> Entities { get; set; }
    public List<Feature> Features { get; set; }

    public LoadedLocationData(Tile[] mapData, int width, int height, List<Entity> entities, List<Feature> features)
    {
        MapData = mapData;
        Width = width;
        Height = height;
        Entities = entities;
        Features = features;
    }
}
