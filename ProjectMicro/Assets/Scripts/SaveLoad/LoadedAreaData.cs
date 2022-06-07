using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedAreaData
{
    public AreaData currentAreaData;
    public AreaData worldData;
    public AreaData[] allLocationData;

    public MapType CurrentMapType { get; set; }
    public int Seed { get; set; }

    public int PlayerWorldX { get; set; }
    public int PlayerWorldY { get; set; }

    public LoadedAreaData(
        AreaData currentAreaData, AreaData worldData,
        AreaData[] allLocationData, MapType currentMapType,
        int seed, int playerWorldX, int playerWorldY)
    {
        this.currentAreaData = currentAreaData;
        this.worldData = worldData;
        this.allLocationData = allLocationData;
        CurrentMapType = currentMapType;
        Seed = seed;
        PlayerWorldX = playerWorldX;
        PlayerWorldY = playerWorldY;
    }
}
