using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates gameobjects to display sprites based on location data
/// </summary>
public class SpriteDisplay : MonoBehaviour
{
    private TileSpriteDisplay tileSpriteDisplay;
    private FeatureSpriteDisplay featureSpriteDisplay;
    private EntitySpriteDisplay entitySpriteDisplay;

    private void Awake()
    {
        GetSpriteDisplayComponents();

        FindObjectOfType<WorldGenerator>()
            .RegisterOnWorldCreated(DisplayInitialMap);
        FindObjectOfType<LocationGenerator>()
            .RegisterOnLocationCreated(DisplayInitialMap);
    }

    private void GetSpriteDisplayComponents()
    {
        tileSpriteDisplay = GetComponent<TileSpriteDisplay>();
        featureSpriteDisplay = GetComponent<FeatureSpriteDisplay>();
        entitySpriteDisplay = GetComponent<EntitySpriteDisplay>();
    }

    private void DisplayInitialMap()
    {
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        // Loop through all tiles in the mapData by x,y
        for (int x = 0; x < areaData.Width; x++)
        {
            for (int y = 0; y < areaData.Height; y++)
            {
                int i = areaData.GetIndexFromCoord(x, y);

                // Place initial tiles
                tileSpriteDisplay.PlaceInitialTile(
                    areaData.MapData[i], x, y);

                // Place initial entities
                if (areaData.MapData[i].entity != null)
                {
                    entitySpriteDisplay.PlaceInitialEntity(
                        areaData.MapData[i].entity, x, y);
                }

                // Place initial features
                if (areaData.MapData[i].TileFeature != null)
                {
                    featureSpriteDisplay.PlaceInitialFeature(
                        areaData.MapData[i].TileFeature, x, y);
                }
            }
        }
    }

    public void ClearAll()
    {
        tileSpriteDisplay.ClearAll();
        featureSpriteDisplay.ClearAll();
        entitySpriteDisplay.ClearAll();
    }

}