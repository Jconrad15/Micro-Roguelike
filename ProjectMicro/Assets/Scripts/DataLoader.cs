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
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        ClearAllOldData();
        CurrentMapType.SetCurrentMapType(loadedAreaData.MapType);

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

        // Tell the generator that data is loaded
        if (loadedAreaData.MapType == MapType.World)
        {
            FindObjectOfType<WorldGenerator>().OnDataLoaded();
        }
        else
        {
            FindObjectOfType<LocationGenerator>().OnDataLoaded();
        }
    }

    public static void ClearAllOldData()
    {
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        foreach (Entity entity in areaData.Entities)
        {
            entity.ClearData();
        }
        FindObjectOfType<AIController>().ClearAll();
        FindObjectOfType<SpriteDisplay>().ClearAll();

        //Clear old Area data from both world and location
        WorldData.Instance.ClearAll();
        LocationData.Instance.ClearAll();
    }
}