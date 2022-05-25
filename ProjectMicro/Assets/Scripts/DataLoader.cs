using UnityEngine;

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

        areaData.SetEntityList(loadedAreaData.Entities);
        areaData.SetFeatureList(loadedAreaData.Features);
        areaData.SetTileNeighbors();

        Entity playerEntity = null;
        // Load entities
        for (int i = 0; i < areaData.Entities.Count; i++)
        {
            if (areaData.Entities[i].type == EntityType.Player)
            {
                playerEntity = areaData.Entities[i];
            }
            else
            {
                AIEntityInstantiation.LoadEntity(
                    areaData.Entities[i] as AIEntity);
            }
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
            entity.Destroy();
        }
        FindObjectOfType<AIController>().ClearAll();
        FindObjectOfType<SpriteDisplay>().ClearAll();
    }
}