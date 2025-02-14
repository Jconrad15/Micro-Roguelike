using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpriteDisplay : MonoBehaviour
{
    private GameObject tilesContainer;
    private Dictionary<Tile, GameObject> placedTiles;
    private SpriteDatabase spriteDatabase;

    [SerializeField]
    private GameObject tilePrefab;

    [SerializeField]
    private VisibilityAlphaChanger visibilityAlphaChanger;

    private void Awake()
    {
        spriteDatabase = FindObjectOfType<SpriteDatabase>();
    }

    private void OnEnable()
    {
        tilesContainer = new GameObject("Tiles");
        tilesContainer.transform.parent = transform;
        placedTiles = new Dictionary<Tile, GameObject>();
    }

    public void PlaceInitialTile(Tile tile, int x, int y)
    {
        if (spriteDatabase.TileDatabase
            .TryGetValue(tile.Type, out Sprite[] s) == false)
        {
            Debug.LogError("No sprites for this tile type");
            return;
        }

        CreateTileGO(tile, x, y, s,
            out GameObject tile_GO, out SpriteRenderer sr);

        // Visibility
        visibilityAlphaChanger.ChangeVisibilityAlpha(tile, sr);

        placedTiles.Add(tile, tile_GO);

        tile.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private void CreateTileGO(
    Tile tile, int x, int y, Sprite[] s,
    out GameObject tile_GO, out SpriteRenderer sr)
    {
        // Determine which sprite from the tileDatabase to use
        int selectedSpriteIndex = DetermineSprite(tile, s);

        tile_GO = Instantiate(tilePrefab, tilesContainer.transform);
        tile_GO.transform.position = new Vector2(x, y);
        sr = tile_GO.GetComponent<SpriteRenderer>();

        sr.sprite = s[selectedSpriteIndex];
    }

    private void OnVisiblityChanged(Tile t)
    {
        if (placedTiles.TryGetValue(t, out GameObject tile_GO))
        {
            visibilityAlphaChanger.ChangeVisibilityAlpha(
                t, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What tile is this?" +
                "Not in tile-GO dictionary.");
        }
    }

    private static int DetermineSprite(Tile tile, Sprite[] s)
    {
        int selectedSpriteIndex = 0;
        // Determine which sprite to use

        // Return 0 if there is only 1 sprite available
        if (s.Length == 1) { return selectedSpriteIndex; }
        
        if (tile.Type == TileType.Water)
        {
            selectedSpriteIndex = DetermineWaterSprite(tile);
        }
        else
        {
            // Randomly choose sprite
            Random.State oldState = Random.state;
            Random.InitState(
                GameInitializer.Instance.Seed + tile.x + tile.y);

            selectedSpriteIndex = Random.Range(0, s.Length);

            Random.state = oldState;
        }

        return selectedSpriteIndex;
    }

    private static int DetermineWaterSprite(Tile tile)
    {
        int selectedSpriteIndex;
        // WATER SPRITE ORDER DETERMINED BY SCRIPTABLE OBJECT

        // Need to check neighbor types to place water
        bool[] isNeighborWater = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            if (tile.neighbors[i] != null)
            {
                if (tile.neighbors[i].Type == TileType.Water)
                {
                    isNeighborWater[i] = true;
                }
            }
        }

        // If water to north, then selectedSpriteIndex must be:
        // 3,4,5,6,7,8,9,13
        if (isNeighborWater[0])
        {
            // If water to East, then selectedSpriteIndex must be:
            // 3,4,6,7
            if (isNeighborWater[1])
            {
                // If water to South, then selectedSpriteIndex must be:
                // 3,4
                if (isNeighborWater[2])
                {
                    // If water to West, then selectedSpriteIndex must be:
                    // 4
                    if (isNeighborWater[3])
                    {
                        //NESW
                        // Water is on all sides of the tile,
                        // can select between different options
                        // Randomly choose sprite
                        Random.State oldState = Random.state;
                        Random.InitState(
                            GameInitializer.Instance.Seed + tile.x + tile.y);

                        if (Random.value > 0.5f)
                        {
                            selectedSpriteIndex = 4;
                        }
                        else
                        {
                            selectedSpriteIndex = 16;
                        }

                        Random.state = oldState;
                    }
                    else
                    {
                        //NES
                        selectedSpriteIndex = 3;
                    }
                }
                else
                {
                    // N,E, not S
                    // If water to West, then selectedSpriteIndex must be:
                    // 6,7
                    if (isNeighborWater[3])
                    {
                        //NEW
                        selectedSpriteIndex = 7;
                    }
                    else
                    {
                        //NE
                        selectedSpriteIndex = 6;
                    }
                }
            }
            else
            {
                // N, not E
                // If water to South, then selectedSpriteIndex must be:
                // 5,13
                if (isNeighborWater[2])
                {
                    // If water to West, then selectedSpriteIndex must be:
                    // 5
                    if (isNeighborWater[3])
                    {
                        //NSW
                        selectedSpriteIndex = 5;
                    }
                    else
                    {
                        //NS
                        selectedSpriteIndex = 13;
                    }
                }
                else
                {
                    // N, not E, not S
                    // If water to West, then selectedSpriteIndex must be:
                    // 8
                    if (isNeighborWater[3])
                    {
                        //NW
                        selectedSpriteIndex = 8;
                    }
                    else
                    {
                        //N
                        selectedSpriteIndex = 9;
                    }
                }
            }
        }
        else
        {
            // Not N
            // If water to East, then selectedSpriteIndex must be:
            // 0,1,10,14
            if (isNeighborWater[1])
            {
                // Not N, E,
                // If water to South, then selectedSpriteIndex must be:
                // 0,1
                if (isNeighborWater[2])
                {
                    // Not N, E, S
                    // If water to West, then selectedSPriteINdex must be:
                    // 1
                    if (isNeighborWater[3])
                    {
                        // ESW
                        selectedSpriteIndex = 1;
                    }
                    else
                    {
                        // ES
                        selectedSpriteIndex = 0;
                    }
                }
                else
                {
                    // Not N, E, Not S
                    // If water to West, then selectedSpriteIndex must be:
                    // 14
                    if (isNeighborWater[3])
                    {
                        //EW
                        selectedSpriteIndex = 14;
                    }
                    else
                    {
                        //E
                        selectedSpriteIndex = 10;
                    }
                }
            }
            else
            {
                // Not N, not E
                // If water to South, then the selectedSpriteIndex must be:
                // 2, 11
                if (isNeighborWater[2])
                {
                    //If water to West, then selectedSpriteINdex must be:
                    // 2
                    if (isNeighborWater[3])
                    {
                        //SW
                        selectedSpriteIndex = 2;
                    }
                    else
                    {
                        //S
                        selectedSpriteIndex = 11;
                    }
                }
                else
                {
                    //Not N, Not E, Not S
                    if (isNeighborWater[3])
                    {
                        // W
                        selectedSpriteIndex = 12;
                    }
                    else
                    {
                        // No surrounding water
                        selectedSpriteIndex = 15;
                    }
                }
            }
        }

        return selectedSpriteIndex;
    }

    public void ClearAll()
    {
        List<GameObject> currentGOs = new List<GameObject>();

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

        // Unregister from each object
        foreach (Tile tile in placedTiles.Keys)
        {
            tile.UnregisterOnVisibilityChanged(OnVisiblityChanged);
        }

        // Clear dictionary
        placedTiles = new Dictionary<Tile, GameObject>();

    }

}
