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
        ResetAllOldData();
        AreaDataManager.Instance.LoadAreaData(loadedAreaData);

        // Load the things for the current area
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        Entity playerEntity = null;
        List<Entity> AIEntitiesToLoad = new List<Entity>();
        // Load entities
        for (int i = 0; i < areaData.Entities.Count; i++)
        {
            if (areaData.Entities[i].type == EntityType.Player)
            {
                playerEntity = areaData.Entities[i];
            }
            else
            {
                // Determine which type of AIEntity is loaded
                AIEntitiesToLoad.Add(areaData.Entities[i]);
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
            loadedAreaData.CurrentMapType, loadedAreaData.Seed);
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