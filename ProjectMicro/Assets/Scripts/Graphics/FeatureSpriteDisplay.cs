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
                        if (feature.T.neighbors[i].TileFeature.type == FeatureType.Wall)
                        {
                            isNeighborWall[i] = true;
                        }
                    }
                }
            }

            // If wall to north, then selectedSpriteIndex must be:
            // 2, 3, 4, 5, 7, 8
            if (isNeighborWall[0])
            {
                // If wall to East, then selectedSpriteIndex must be:
                // 2, 7
                if (isNeighborWall[1])
                {
                    // If wall to South, then selectedSpriteIndex must be:
                    // 7
                    if (isNeighborWall[2])
                    {
                        selectedSpriteIndex = 7;
                    }
                    else
                    {
                        selectedSpriteIndex = 2;
                    }
                }
                else
                {
                    // If wall to South, then selectedSpriteIndex must be:
                    // 3, 4, 8
                    if (isNeighborWall[2])
                    {
                        // If wall to West, then selectedSpriteIndex must be:
                        // 8
                        if (isNeighborWall[3])
                        {
                            selectedSpriteIndex = 8;
                        }
                        else
                        {
                            // Need to determine between two types of north-south walls
                            // Check what type of wall is to the north
                            bool isDetermined = false;
                            Tile currentTile = feature.T;
                            while (isDetermined == false)
                            {
                                Tile[] neighborNeighbors =
                                    currentTile.neighbors[0].neighbors;

                                // Check northern neighbor's eastern neighbor
                                if (neighborNeighbors[1].TileFeature != null)
                                {
                                    if (neighborNeighbors[1].TileFeature.type == FeatureType.Wall ||
                                        neighborNeighbors[1].TileFeature.type == FeatureType.Door)
                                    {
                                        selectedSpriteIndex = 3;
                                        isDetermined = true;
                                        continue;
                                    }
                                }
                                // Check northern neighbor's western neighbor
                                else if (neighborNeighbors[3].TileFeature != null)
                                {
                                    if (neighborNeighbors[3].TileFeature.type == FeatureType.Wall ||
                                        neighborNeighbors[3].TileFeature.type == FeatureType.Door)
                                    {
                                        selectedSpriteIndex = 4;
                                        isDetermined = true;
                                        continue;
                                    }
                                }
                                else if (currentTile.neighbors[0].TileFeature != null)
                                {
                                    // if northern tile is not a wall, then set this 
                                    // tile to be east west wall
                                    if (currentTile.neighbors[0].TileFeature.type != FeatureType.Wall ||
                                        currentTile.neighbors[0].TileFeature.type != FeatureType.Door)
                                    {
                                        selectedSpriteIndex = 1;
                                        isDetermined = true;
                                        continue;
                                    }
                                }

                                // Need to look north again using the
                                // northern neighbor as the staring point

                                // TODO: make better decision
                                // also check for null
                                if (neighborNeighbors[0] == null)
                                {
                                    // just select 3
                                    selectedSpriteIndex = 3;
                                }

                                currentTile = currentTile.neighbors[0];
                            }
                        }
                    }
                    else
                    {
                        // If wall to West, then selectedSpriteIndex must be:
                        // 5
                        if (isNeighborWall[3])
                        {
                            selectedSpriteIndex = 5;
                        }
                        else
                        {
                            selectedSpriteIndex = 2;
                        }
                    }
                }
            }
            else
            {
                // If wall to East, then selectedSpriteIndex must be:
                // 0, 1
                if (isNeighborWall[1])
                {
                    // If wall to South, then selectedSpriteIndex must be:
                    // 0
                    if (isNeighborWall[2])
                    {
                        selectedSpriteIndex = 0;
                    }
                    else
                    {
                        selectedSpriteIndex = 1;
                    }
                }
                else
                {
                    // If wall to South, then the selectedSpriteIndex must be:
                    // 6
                    if (isNeighborWall[2])
                    {
                        selectedSpriteIndex = 6;
                    }
                    else
                    {
                        selectedSpriteIndex = 1;
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
