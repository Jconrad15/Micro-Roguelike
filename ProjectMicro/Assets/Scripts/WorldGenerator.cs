using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private int seed;

    private Action cbOnWorldCreated;
    private Action<Player> cbOnPlayerCreated;
    private Action<AIEntity> cbOnAIEntityCreated;

    private ItemDatabase itemDB;

    void Start()
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        InitializeItemDatabase();

        CreateMapData();
        CreateFeatures();
        CreatePlayer();
        CreateAIEntities();

        Random.state = oldState;

        cbOnWorldCreated?.Invoke();
    }

    private void InitializeItemDatabase()
    {
        itemDB = FindObjectOfType<ItemDatabase>();
        itemDB.CreateDatabase();
    }

    private void CreatePlayer()
    {
        Tile playerTile = WorldData.Instance.GetTile(width / 2, 0);
        Player player = new Player(playerTile, EntityType.Player);
        playerTile.entity = player;

        // Starting player items
        player.InventoryItems.Add(GenerateRandomItem());

        cbOnPlayerCreated?.Invoke(player);
    }

    private Item GenerateRandomItem()
    {
        // TODO: store this items list? so that it isn't created each time
        List<Item> items = new List<Item>(itemDB.ItemRefDatabase.Values);
        return items[Random.Range(0, items.Count)];
    }

    private void CreateAIEntities()
    {
        // For now generate a dog
        Tile dogTile = WorldData.Instance.GetTile(1, 1);
        AIEntity dog = new AIEntity(dogTile, EntityType.Dog);
        dogTile.entity = dog;
        dog.InventoryItems.Add(GenerateRandomItem());
        cbOnAIEntityCreated?.Invoke(dog);

        // Also create a trader
        Tile traderTile = WorldData.Instance.GetTile(2, 1);
        AIEntity trader = new AIEntity(traderTile, EntityType.Trader);
        traderTile.entity = trader;
        trader.InventoryItems.Add(GenerateRandomItem());
        cbOnAIEntityCreated.Invoke(trader);
    }

    private void CreateMapData()
    {
        WorldData.Instance.MapData = new Tile[width * height];
        WorldData.Instance.Width = width;
        WorldData.Instance.Height = height;

        TileType[] rawMap = CreateRawMapData();

        // Set tile types
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            // For now just generate walls here
            if ((x == 5 && y >= 5 && y <= 10) ||
                y == 10 && x >= 0 && x <= 10 ||
                y == 10 && x >= 12 && x <= 30)
            {
                WorldData.Instance.MapData[i] = new Tile(x, y, TileType.Wall);
                continue;
            }

            // Otherwise set to open tile
            WorldData.Instance.MapData[i] = new Tile(x, y, rawMap[i]);
        }

        SetTileNeighbors();
        WorldData.Instance.GenerateTileGraph();
    }

    private TileType[] CreateRawMapData()
    {
        TileType[] rawMap = new TileType[width * height];

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            float sample = Random.Range(0f, 1f);
            if (sample <= 0.1f)
            {
                rawMap[i] = TileType.Water;
            }
            else
            {
                rawMap[i] = TileType.OpenArea;
            }
        }

        return rawMap;
    }

    private void CreateFeatures()
    {
        Tile[] mapdata = WorldData.Instance.MapData;
        for (int i = 0; i < mapdata.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            if (x == 11 && y == 10)
            {
                mapdata[i].feature = new Feature(FeatureType.Door, mapdata[i]);
            }
        }
    }

    private static void SetTileNeighbors()
    {
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            Tile[] neighbors =
                WorldData.Instance.GetNeighboringTiles(
                    WorldData.Instance.MapData[i]);
            WorldData.Instance.MapData[i].SetNeighbors(neighbors);
        }
    }

    public void OnDataLoaded(List<Entity> loadedEntities)
    {
        SetTileNeighbors();

        for (int i = 0; i < loadedEntities.Count; i++)
        {
            if (loadedEntities[i].type == EntityType.Player)
            {
                cbOnPlayerCreated?.Invoke(loadedEntities[i] as Player);
            }
            else
            {
                cbOnAIEntityCreated?.Invoke(loadedEntities[i] as AIEntity);
            }
        }

        cbOnWorldCreated?.Invoke();
    }

    public void RegisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldCreated += callbackfunc;
    }

    public void UnregisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldCreated -= callbackfunc;
    }

    public void RegisterOnPlayerCreated(Action<Player> callbackfunc)
    {
        cbOnPlayerCreated += callbackfunc;
    }

    public void UnregisterOnPlayerCreated(Action<Player> callbackfunc)
    {
        cbOnPlayerCreated -= callbackfunc;
    }

    public void RegisterOnAIEntityCreated(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated += callbackfunc;
    }

    public void UnregisterOnAIEntityCreated(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated -= callbackfunc;
    }
}
