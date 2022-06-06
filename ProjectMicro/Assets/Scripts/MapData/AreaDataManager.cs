using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum MapType { World, Location };
public class AreaDataManager : MonoBehaviour
{
    public static AreaDataManager Instance { get; private set; }
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

    private static Action<MapType> cbOnCurrentMapTypeChange;

    public MapType CurrentMapType { get; private set; }

    public void SetCurrentMapType(MapType type)
    {
        if (CurrentMapType != type)
        {
            cbOnCurrentMapTypeChange?.Invoke(type);
        }

        CurrentMapType = type;
    }

    public AreaData CurrentLocationData;
    private AreaData WorldData;

    private AreaData[] AllLocationData;

    public void SetWorldData(AreaData worldData)
    {
        WorldData = worldData;
    }

    public AreaData GetWorldData()
    {
        return WorldData;
    }

    public void Initiate(int worldWidth, int worldHeight)
    {
        CurrentLocationData = new AreaData();
        WorldData = new AreaData();

        AllLocationData = new AreaData[worldWidth * worldHeight];
    }

    public void StoreLocationData()
    {
        (int worldX, int worldY) = 
            WorldGenerator.Instance.GetSavedPlayerWorldPosition();
        // Get the world index for this location area data
        int index = WorldData.GetIndexFromCoord(worldX, worldY);

        // When stored, need to remove old player from tile location's entity
        Player player = FindObjectOfType<PlayerController>().GetPlayer();
        player.T.entity = null;
        CurrentLocationData.Entities.Remove(player);

        // TODO: disconnect this UI reference
        FindObjectOfType<DialogueUI>().UnregisterToClicksOnEntities();

        AllLocationData[index] = CurrentLocationData;
        CurrentLocationData = new AreaData();
    }

    public bool TryGetLocationData(int worldX, int worldY,
        out AreaData locationData)
    {
        locationData = null;

        int index = WorldData.GetIndexFromCoord(worldX, worldY);

        if (index < 0 || index >= AllLocationData.Length) { return false; }

        locationData = AllLocationData[index];
        if (locationData == null) { return false; }
        return true;
    }

    public void RegisterOnCurrentMapTypeChange(
        Action<MapType> callbackfunc)
    {
        cbOnCurrentMapTypeChange += callbackfunc;
    }

    public void UnregisterOnCurrentMapTypeChange(
        Action<MapType> callbackfunc)
    {
        cbOnCurrentMapTypeChange -= callbackfunc;
    }
}
