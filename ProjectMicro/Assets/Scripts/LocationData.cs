using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton holding location data
/// </summary>
public class LocationData : AreaData
{
    public static LocationData Instance { get; private set; }
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
        SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
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
