using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureSpriteDisplay : MonoBehaviour
{
    private GameObject featuresContainer;
    private Dictionary<Feature, GameObject> placedFeatures;
    private SpriteDatabase spriteDatabase;

    [SerializeField]
    private GameObject featurePrefab;

    [SerializeField]
    private VisibilityAlphaChanger visibilityAlphaChanger;

    private void Awake()
    {
        spriteDatabase = FindObjectOfType<SpriteDatabase>();
    }

    private void OnEnable()
    {
        featuresContainer = new GameObject("Features");
        featuresContainer.transform.parent = transform;
        placedFeatures = new Dictionary<Feature, GameObject>();
    }

    public void PlaceInitialFeature(Feature feature, int x, int y)
    {
        spriteDatabase.FeatureDatabase
            .TryGetValue(feature.type, out Sprite[] s);

        CreateFeatureGO(feature, x, y, s,
            out GameObject newFeature, out SpriteRenderer sr);

        // Visibility
        visibilityAlphaChanger.ChangeVisibilityAlpha(feature, sr);

        placedFeatures.Add(feature, newFeature);

        feature.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private void CreateFeatureGO(
        Feature feature, int x, int y, Sprite[] s,
        out GameObject newFeature, out SpriteRenderer sr)
    {
        newFeature =
            Instantiate(featurePrefab, featuresContainer.transform);
        newFeature.transform.position = new Vector2(x, y);

        sr = newFeature.GetComponent<SpriteRenderer>();
        int selectedSpriteIndex = DetermineSprite(feature);
        sr.sprite = s[selectedSpriteIndex];
    }

    private static int DetermineSprite(Feature feature)
    {
        if (feature == null) { return -1; }

        int selectedSpriteIndex = 0;
        if (feature.type == FeatureType.Wall)
        {
            // WALL SPRITE ORDER DETERMINED BY SCRIPTABLE OBJECT

            // Need to check neighbor types to place a wall
            bool[] isNeighborWall = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                if (feature.T.neighbors[i] != null)
                {
                    if (feature.T.neighbors[i].TileFeature != null)
                    {
                        if (feature.T.neighbors[i].TileFeature.type
                            == FeatureType.Wall)
                        {
                            isNeighborWall[i] = true;
                        }
                    }
                }
            }

            if (isNeighborWall[0])
            {
                // Wall to north

                if (isNeighborWall[1])
                {
                    // Wall to north and east

                    if (isNeighborWall[2])
                    {
                        // Wall to north, east, and south
                        
                        if (isNeighborWall[3])
                        {
                            // Wall to north, east, south, and west
                            selectedSpriteIndex = 7;
                        }
                        else
                        {
                            // Wall to north, east, south, no wall to west
                            selectedSpriteIndex = 4;
                        }
                    }
                    else
                    {
                        // Wall to north and east, no wall to south

                        if (isNeighborWall[3])
                        {
                            // Wall to north, east, west, no wall south
                            selectedSpriteIndex = 5;
                        }
                        else
                        {
                            // Wall to north, east, no wall south and west
                            selectedSpriteIndex = 1;
                        }
                    }
                }
                else
                {
                    // Wall to north, no wall to east
                    
                    if (isNeighborWall[2])
                    {
                        // Wall to north and south, no wall to east

                        if (isNeighborWall[3])
                        {
                            // Wall to north, south, west, no wall east
                            selectedSpriteIndex = 6;
                        }
                        else
                        {
                            // Wall to north and south, no wall east, west
                            selectedSpriteIndex = 3;
                        }
                    }
                    else
                    {
                        // Wall to north, no wall to east or south

                        if (isNeighborWall[3])
                        {
                            // Wall to north, west, no wall east, south
                            selectedSpriteIndex = 2;
                        }
                        else
                        {
                            // Wall to north, no wall to east, south, west
                            selectedSpriteIndex = 0;
                        }
                    }
                }
            }
            else
            {
                // No wall to north

                if (isNeighborWall[1])
                {
                    // Wall east, no wall to north

                    if (isNeighborWall[2])
                    {
                        // Wall to east, and south, no wall to north

                        if (isNeighborWall[3])
                        {
                            // Wall east, south, and west, no wall to north
                            selectedSpriteIndex = 11;
                        }
                        else
                        {
                            // Wall east, south, no wall to north, west
                            selectedSpriteIndex = 9;
                        }
                    }
                    else
                    {
                        // Wall to east, no wall to north, south

                        if (isNeighborWall[3])
                        {
                            // Wall to east, west, no wall north south
                            selectedSpriteIndex = 10;
                        }
                        else
                        {
                            // Wall to east, no wall north, south and west
                            selectedSpriteIndex = 8;
                        }
                    }
                }
                else
                {
                    // No wall to north, east

                    if (isNeighborWall[2])
                    {
                        // Wall tosouth, no wall to north east

                        if (isNeighborWall[3])
                        {
                            // Wall to south, west, no wall north east
                            selectedSpriteIndex = 13;
                        }
                        else
                        {
                            // Wall to south, no wall north, east, west
                            selectedSpriteIndex = 12;
                        }
                    }
                    else
                    {
                        // No wall to north east or south

                        if (isNeighborWall[3])
                        {
                            // Wall to west, no wall north, east, south
                            selectedSpriteIndex = 14;
                        }
                        else
                        {
                            // No wall to north, east, south, west
                            selectedSpriteIndex = 15;
                        }
                    }
                }
            }

        }

        return selectedSpriteIndex;
    }

    private void OnVisiblityChanged(Feature f)
    {
        if (placedFeatures.TryGetValue(f, out GameObject tile_GO))
        {
            visibilityAlphaChanger.ChangeVisibilityAlpha(
                f, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What Feature is this?" +
                "Not in entity-GO dictionary.");
        }
    }

    public void ClearAll()
    {
        List<GameObject> currentGOs = new List<GameObject>();

        foreach (GameObject go in placedFeatures.Values)
        {
            currentGOs.Add(go);
        }

        // Destroy all placed gameobjects
        foreach (GameObject go in currentGOs)
        {
            Destroy(go);
        }
        StopAllCoroutines();

        // Unregister from each object
        foreach (Feature feature in placedFeatures.Keys)
        {
            feature.UnregisterOnVisibilityChanged(OnVisiblityChanged);
        }

        // Clear dictionary
        placedFeatures = new Dictionary<Feature, GameObject>();
    }
}
