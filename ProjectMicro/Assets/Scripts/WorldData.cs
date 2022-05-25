using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MapType { World, Location };

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
        SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
    }

    /// <summary>
    /// Sets each tile's list of neighbors. Used when create the tile array.
    /// </summary>
    public void SetTileNeighbors()
    {
        for (int i = 0; i < Instance.MapData.Length; i++)
        {
            Tile[] neighbors =
                Instance.GetNeighboringTiles(
                    Instance.MapData[i]);
            Instance.MapData[i].SetNeighbors(neighbors);
        }
    }

}
