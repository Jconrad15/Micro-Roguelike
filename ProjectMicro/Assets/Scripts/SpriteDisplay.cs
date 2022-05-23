using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Generates gameobjects to display sprites based on world data
/// </summary>
public class SpriteDisplay : MonoBehaviour
{
    private WorldGenerator worldGenerator;
    private SpriteDatabase spriteDatabase;
    private GameObject tilesContainer;
    private GameObject entitiesContainer;
    private GameObject featuresContainer;

    private readonly float moveSpeed = 10f;
    private readonly float alphaChangeSpeed = 0.3f;

    private Dictionary<Tile, GameObject> placedTiles =
        new Dictionary<Tile, GameObject>();

    private Dictionary<Entity, GameObject> placedEntities =
        new Dictionary<Entity, GameObject>();

    private Dictionary<Feature, GameObject> placedFeatures =
        new Dictionary<Feature, GameObject>();

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject entitiesPrefab;
    [SerializeField]
    private GameObject featurePrefab;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.RegisterOnWorldCreated(DisplayInitialWorld);

        spriteDatabase = FindObjectOfType<SpriteDatabase>();

        tilesContainer = new GameObject("Tiles");
        tilesContainer.transform.parent = transform;

        entitiesContainer = new GameObject("Entities");
        entitiesContainer.transform.parent = transform;

        featuresContainer = new GameObject("Features");
        featuresContainer.transform.parent = transform;
    }

    private void DisplayInitialWorld()
    {
        Tile[] mapData = WorldData.Instance.MapData;
        int width = WorldData.Instance.Width;
        int height = WorldData.Instance.Height;

        // Create the sprite database if it does not yet exist
        if (spriteDatabase.TileDatabase == null ||
            spriteDatabase.EntityDatabase == null)
        {
            spriteDatabase.CreateDatabases();
        }

        // Loop through all tiles in the mapData by x,y
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int i = WorldData.Instance.GetIndexFromCoord(x, y);

                // Place initial tiles
                PlaceInitialTile(mapData[i], x, y);

                // Place initial entities
                if (mapData[i].entity != null)
                {
                    PlaceInitialEntity(mapData[i].entity, x, y);
                }

                // Place initial features
                if (mapData[i].feature != null)
                {
                    PlaceInitialFeature(mapData[i].feature, x, y);
                }
            }
        }
    }

    private void PlaceInitialFeature(Feature feature, int x, int y)
    {
        if (spriteDatabase.EntityDatabase == null)
        {
            Debug.LogError("No entity sprite database yet");
        }

        spriteDatabase.FeatureDatabase.TryGetValue(feature.type, out Sprite[] s);

        GameObject newFeature = Instantiate(featurePrefab, featuresContainer.transform);
        newFeature.transform.position = new Vector2(x, y);

        SpriteRenderer sr = newFeature.GetComponent<SpriteRenderer>();
        // TODO: determine which sprite to use
        // For now just use the first sprite
        sr.sprite = s[0];

        // Visibility
        ChangeVisibilityAlpha(feature, sr);

        placedFeatures.Add(feature, newFeature);

        feature.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private void PlaceInitialEntity(Entity entity, int x, int y)
    {
        if (spriteDatabase.EntityDatabase == null)
        {
            Debug.LogError("No entity sprite database yet");
        }

        spriteDatabase.EntityDatabase.TryGetValue(entity.type, out Sprite s);

        GameObject newTile = Instantiate(entitiesPrefab, entitiesContainer.transform);
        newTile.transform.position = new Vector2(x, y);

        SpriteRenderer sr = newTile.GetComponent<SpriteRenderer>();
        sr.sprite = s;

        // Set entity in the trader component in the prefabs
        newTile.GetComponent<Trader>().SetEntity(entity);

        // Visibility
        ChangeVisibilityAlpha(entity, sr);

        placedEntities.Add(entity, newTile);

        entity.RegisterOnMove(OnEntityMove);
        entity.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private void PlaceInitialTile(Tile tile, int x, int y)
    {
        if (spriteDatabase.TileDatabase == null)
        {
            Debug.LogError("No tile sprite database yet");
        }

        if (spriteDatabase.TileDatabase
            .TryGetValue(tile.type, out Sprite[] s) == false)
        {
            Debug.LogError("No sprites for this tile type");
            return;
        }

        // Determine which sprite from the tileDatabase to use
        int selectedSpriteIndex = DetermineSprite(tile);

        GameObject tile_GO = Instantiate(tilePrefab, tilesContainer.transform);
        tile_GO.transform.position = new Vector2(x, y);
        SpriteRenderer sr = tile_GO.GetComponent<SpriteRenderer>();

        if (selectedSpriteIndex == -1)
        {
            Debug.LogError("Why was no sprite index selected?");
        }
        sr.sprite = s[selectedSpriteIndex];

        // Visibility
        ChangeVisibilityAlpha(tile, sr);

        placedTiles.Add(tile, tile_GO);

        tile.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private static int DetermineSprite(Tile tile)
    {
        int selectedSpriteIndex = -1;
        // Determine which sprite to use
        if (tile.type == TileType.Wall)
        {
            // WALL SPRITE ORDER DETERMINED BY SCRIPTABLE OBJECT

            // Need to check neighbor types to place a wall
            bool[] isNeighborWall = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                if (tile.neighbors[i] != null)
                {
                    if (tile.neighbors[i].type == TileType.Wall)
                    {
                        isNeighborWall[i] = true;
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
                            Tile currentTile = tile;
                            while (isDetermined == false)
                            {
                                Tile[] neighborNeighbors =
                                    currentTile.neighbors[0].neighbors;

                                // Check northern neighbor's eastern neighbor
                                if (neighborNeighbors[1].type == TileType.Wall)
                                {
                                    selectedSpriteIndex = 3;
                                    isDetermined = true;
                                }
                                // Check northern neighbor's western neighbor
                                else if (neighborNeighbors[3].type == TileType.Wall)
                                {
                                    selectedSpriteIndex = 4;
                                    isDetermined = true;
                                }
                                else if (currentTile.neighbors[0].type != TileType.Wall)
                                {
                                    // if northern tile is not a wall, then set this 
                                    // tile to be east west wall

                                    selectedSpriteIndex = 1;
                                    isDetermined = true;
                                }
                                else
                                {
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
        else
        {
            selectedSpriteIndex = 0;
        }

        return selectedSpriteIndex;
    }

    private Dictionary<SpriteRenderer, Coroutine> currentlyLerpingSRs =
        new Dictionary<SpriteRenderer, Coroutine>();

    private void ChangeVisibilityAlpha(Feature feature, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)feature.Visibility / 2f);

    }

    private void ChangeVisibilityAlpha(Tile tile, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)tile.Visibility / 2f);

    }

    private void ChangeVisibilityAlpha(Entity entity, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)entity.Visibility / 2f);
    }

    private void ChangeVisibilityAlpha(SpriteRenderer sr, float targetAlpha)
    {
        if (currentlyLerpingSRs.ContainsKey(sr))
        {
            currentlyLerpingSRs.TryGetValue(sr, out Coroutine runningCR);
            if (runningCR != null)
            {
                StopCoroutine(runningCR);
            }
            currentlyLerpingSRs.Remove(sr);
        }

        Coroutine cr = StartCoroutine(LerpVisibility(sr, targetAlpha));
        currentlyLerpingSRs.Add(sr, cr);
    }

    private IEnumerator LerpVisibility(SpriteRenderer sr, float targetAlpha)
    {
        Color currentColor = sr.color;
        Color targetColor = currentColor;
        targetColor.a = targetAlpha;
        float t = 0;
        while (Mathf.Abs(currentColor.a - targetColor.a) > 0.01)
        {
            currentColor.a = Mathf.Lerp(currentColor.a, targetColor.a, t);
            sr.color = currentColor;
            t += alphaChangeSpeed * Time.deltaTime;
            yield return null;
        }

        currentlyLerpingSRs.Remove(sr);
        sr.color = targetColor;
    }

    private void OnEntityMove(Entity entity, Vector2 startPos)
    {
        if (placedEntities.TryGetValue(entity, out GameObject entity_GO))
        {
            StartCoroutine(SmoothEntityMove(entity, entity_GO, startPos));
        }
    }

    private void OnVisiblityChanged(Feature f)
    {
        if (placedFeatures.TryGetValue(f, out GameObject tile_GO))
        {
            ChangeVisibilityAlpha(f, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What Entity is this? Not in entity-GO dictionary.");
        }
    }

    private void OnVisiblityChanged(Entity e)
    {
        if (placedEntities.TryGetValue(e, out GameObject tile_GO))
        {
            ChangeVisibilityAlpha(e, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What Entity is this? Not in entity-GO dictionary.");
        }
    }

    private void OnVisiblityChanged(Tile t)
    {
        if (placedTiles.TryGetValue(t, out GameObject tile_GO))
        {
            ChangeVisibilityAlpha(t, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What tile is this? Not in tile-GO dictionary.");
        }
    }

    public void ClearAll()
    {
        List<GameObject> currentGOs = new List<GameObject>();
        // Get single list of all placed gameobjects
        foreach (GameObject go in placedEntities.Values)
        {
            currentGOs.Add(go);
        }
        foreach (GameObject go in placedFeatures.Values)
        {
            currentGOs.Add(go);
        }
        foreach (GameObject go in placedTiles.Values)
        {
            currentGOs.Add(go);
        }

        // Destroy all placed gameobjects
        foreach (GameObject go in currentGOs)
        {
            Destroy(go);
        }
        StopAllCoroutines();

        // Clear dictionaries
        placedTiles.Clear();
        placedEntities.Clear();
        placedFeatures.Clear();

    }

    private IEnumerator SmoothEntityMove(
        Entity entity, GameObject entity_GO, Vector2 startPos)
    {
        Vector2 destinationLocation = new Vector2(entity.X, entity.Y);
        Vector2 currentLocation = startPos;

        while (Vector3.Distance(destinationLocation, currentLocation) > 0.001f)
        {
            float step = moveSpeed * Time.deltaTime;
            currentLocation = Vector2.MoveTowards(
                currentLocation, destinationLocation, step);

            entity_GO.transform.position = currentLocation;
            yield return null;
        }

        entity_GO.transform.position = destinationLocation;
    }

}
