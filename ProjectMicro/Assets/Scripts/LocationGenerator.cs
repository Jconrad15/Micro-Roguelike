using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocationGenerator : MonoBehaviour
{
    private int width;
    private int height;
    private int seed;

    private Action cbOnLocationCreated;
    private Action<Player> cbOnPlayerCreated;
    private Action<AIEntity> cbOnAIEntityCreated;

    /// <summary>
    /// Initiate generation of this location.
    /// </summary>
    /// <param name="seed"></param>
    public void StartGenerateLocation(int seed, int width, int height,
        int worldX, int worldY)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed + worldX + worldY);
        this.seed = seed;
        this.width = width;
        this.height = height;

        CreateMapData();
        CreateFeatures();
        CreatePlayer();
        CreateAIEntities();

        Random.state = oldState;

        cbOnLocationCreated?.Invoke();
    }

    private void CreatePlayer()
    {
        // First determine where the player will go
        int playerStartX = width / 2;
        int playerStartY = 0;

        // Get the tile at the location
        Tile playerTile = LocationData.Instance.GetTile(playerStartX, playerStartY);
        
        // Place the player at the tile, and the tile to the player
        Player player = new Player(playerTile, EntityType.Player, 10);
        playerTile.entity = player;

        // TODO: Starting player items
        player.InventoryItems.Add(ItemDatabase.GetRandomItem());

        cbOnPlayerCreated?.Invoke(player);
    }

    /// <summary>
    /// Creates the AI entities in the location.
    /// </summary>
    private void CreateAIEntities()
    {
        // For now generate a dog
        Tile dogTile = LocationData.Instance.GetTile(1, 1);
        Dog dog = new Dog(dogTile, EntityType.AI, 0);
        dogTile.entity = dog;
        cbOnAIEntityCreated?.Invoke(dog);

        // Also create a Merchant
        Tile merchantTile = LocationData.Instance.GetTile(2, 1);
        Merchant merchant = new Merchant(merchantTile, EntityType.AI, 10);
        merchantTile.entity = merchant;
        cbOnAIEntityCreated?.Invoke(merchant);
    }

    private void CreateMapData()
    {
        LocationData.Instance.MapData = new Tile[width * height];
        LocationData.Instance.Width = width;
        LocationData.Instance.Height = height;

        // Create base tile type map
        TileType[] rawMap = CreateRawMapData();

        // Set tile types
        for (int i = 0; i < LocationData.Instance.MapData.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            // Create walls
            // For now just generate walls here
            if ((x == 5 && y >= 5 && y <= 10) ||
                (y == 10 && x >= 0 && x <= 10) ||
                (y == 10 && x >= 12 && x <= 30))
            {
                LocationData.Instance.MapData[i] = new Tile(x, y, TileType.Wall);
                continue;
            }

            // Otherwise set to open tile
            LocationData.Instance.MapData[i] = new Tile(x, y, rawMap[i]);
        }

        LocationData.SetTileNeighbors();
        LocationData.Instance.GenerateTileGraph();
    }

    private TileType[] CreateRawMapData()
    {
        TileType[] rawMap = new TileType[width * height];

        SimplexNoise.Seed = seed;
        float scale = 0.1f;

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            float sample = SimplexNoise.CalcPixel2D(x, y, scale) / 255f;
            if (sample <= 0.2f)
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

    /// <summary>
    /// Creates the features in the location.
    /// </summary>
    private void CreateFeatures()
    {
        Tile[] mapdata = LocationData.Instance.MapData;
        for (int i = 0; i < mapdata.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);

            if (x == 11 && y == 10)
            {
                mapdata[i].feature = new Feature(FeatureType.Door, mapdata[i]);
            }
        }
    }

    public void OnDataLoaded(List<Entity> loadedEntities)
    {
        LocationData.SetTileNeighbors();

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

        cbOnLocationCreated?.Invoke();
    }

    public void RegisterOnLocationCreated(Action callbackfunc)
    {
        cbOnLocationCreated += callbackfunc;
    }

    public void UnregisterOnLocationCreated(Action callbackfunc)
    {
        cbOnLocationCreated -= callbackfunc;
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
