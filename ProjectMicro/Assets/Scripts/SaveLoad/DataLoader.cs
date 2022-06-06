using UnityEngine;
using System.Collections.Generic;

public class DataLoader : MonoBehaviour
{
    private void Start()
    {
        SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
    }

    protected static void OnDataLoaded(LoadedAreaData loadedAreaData)
    {
        AreaData areaData;
        if (loadedAreaData.MapType == MapType.World)
        {
            areaData = AreaDataManager.Instance.GetWorldData();
        }
        else
        {
            areaData = AreaDataManager.Instance.CurrentLocationData;
        }

        ResetAllOldData();
        AreaDataManager.Instance.SetCurrentMapType(loadedAreaData.MapType);

        areaData.Width = loadedAreaData.Width;
        areaData.Height = loadedAreaData.Height;
        areaData.MapData = loadedAreaData.MapData;

        areaData.SetFeatureList(loadedAreaData.Features);
        areaData.SetTileNeighbors();

        Entity playerEntity = null;
        List<Entity> AIEntitiesToLoad = new List<Entity>();
        // Load entities
        for (int i = 0; i < loadedAreaData.Entities.Count; i++)
        {
            if (loadedAreaData.Entities[i].type == EntityType.Player)
            {
                playerEntity = loadedAreaData.Entities[i];
            }
            else
            {
                // Determine which type of AIEntity is loaded
                AIEntitiesToLoad.Add(loadedAreaData.Entities[i]);
            }
        }

        // Load the entities
        for (int i = 0; i < AIEntitiesToLoad.Count; i++)
        {
            AIEntityInstantiation.LoadEntity(AIEntitiesToLoad[i]);
        }

        PlayerInstantiation.LoadPlayer(playerEntity);

        areaData.GenerateTileGraph();

        // Tell the GameInitializer that data is loaded
        GameInitializer.Instance.OnDataLoaded(
            loadedAreaData.MapType, loadedAreaData.Seed);
    }

    public static void ResetAllOldData()
    {
        FindObjectOfType<AIController>().ClearAll();
        FindObjectOfType<SpriteDisplay>().ClearAll();

        // If in location, store the location
        if (AreaDataManager.Instance.CurrentMapType == MapType.Location)
        {
            AreaDataManager.Instance.StoreLocationData();
        }
    }
}