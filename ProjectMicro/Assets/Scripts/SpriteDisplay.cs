using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDisplay : MonoBehaviour
{
    private WorldGenerator worldGenerator;
    private SpriteDatabase spriteDatabase;
    private GameObject tiles;
    private GameObject entities;

    private float moveSpeed = 10f;

    private List<GameObject> placedTiles = new List<GameObject>();

    private Dictionary<Entity, GameObject> placedEntities =
        new Dictionary<Entity, GameObject>();

    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject entitiesPrefab;

    void Start()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.RegisterOnWorldCreated(DisplayInitialWorld);

        spriteDatabase = FindObjectOfType<SpriteDatabase>();

        tiles = new GameObject("Tiles");
        tiles.transform.parent = transform;

        entities = new GameObject("Entities");
        entities.transform.parent = transform;
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
            }
        }
    }

    private void PlaceInitialEntity(Entity entity, int x, int y)
    {
        if (spriteDatabase.EntityDatabase == null)
        {
            Debug.LogError("No entity sprite database yet");
        }

        spriteDatabase.EntityDatabase.TryGetValue(entity.type, out Sprite s);

        GameObject newTile = Instantiate(entitiesPrefab, entities.transform);
        newTile.transform.position = new Vector2(x, y);
        newTile.GetComponent<SpriteRenderer>().sprite = s;
        placedEntities.Add(entity, newTile);

        entity.RegisterOnMove(OnEntityMove);
    }

    private void PlaceInitialTile(Tile tile, int x, int y)
    {
        if (spriteDatabase.TileDatabase == null)
        {
            Debug.LogError("No tile sprite database yet");
        }

        spriteDatabase.TileDatabase.TryGetValue(tile.type, out Sprite s);

        GameObject newTile = Instantiate(tilePrefab, tiles.transform);
        newTile.transform.position = new Vector2(x, y);
        newTile.GetComponent<SpriteRenderer>().sprite = s;
        placedTiles.Add(newTile);
    }

    private void OnEntityMove(Entity entity, Vector2 startPos)
    {
        if (placedEntities.TryGetValue(entity, out GameObject entity_GO))
        {
            StartCoroutine(SmoothEntityMove(entity, entity_GO, startPos));
            //entity_GO.transform.position = new Vector2(entity.X, entity.Y);
        }
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
