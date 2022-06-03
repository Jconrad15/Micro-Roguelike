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
        int selectedSpriteIndex = DetermineSprite(tile);

        tile_GO = Instantiate(tilePrefab, tilesContainer.transform);
        tile_GO.transform.position = new Vector2(x, y);
        sr = tile_GO.GetComponent<SpriteRenderer>();
        if (selectedSpriteIndex == -1)
        {
            Debug.LogError("Why was no sprite index selected?");
        }
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

    private static int DetermineSprite(Tile tile)
    {
        int selectedSpriteIndex = -1;
        // Determine which sprite to use
        if (tile.Type == TileType.Wall)
        {
            // WALL SPRITE ORDER DETERMINED BY SCRIPTABLE OBJECT

            // Need to check neighbor types to place a wall
            bool[] isNeighborWall = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                if (tile.neighbors[i] != null)
                {
                    if (tile.neighbors[i].Type == TileType.Wall)
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
                                if (neighborNeighbors[1].Type == TileType.Wall)
                                {
                                    selectedSpriteIndex = 3;
                                    isDetermined = true;
                                }
                                // Check northern neighbor's western neighbor
                                else if (neighborNeighbors[3].Type == TileType.Wall)
                                {
                                    selectedSpriteIndex = 4;
                                    isDetermined = true;
                                }
                                else if (currentTile.neighbors[0].Type != TileType.Wall)
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
        else if (tile.Type == TileType.Water)
        {
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
                            selectedSpriteIndex = 4;
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
        }
        else
        {
            selectedSpriteIndex = 0;
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
