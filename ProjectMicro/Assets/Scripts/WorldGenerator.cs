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

    private Action cbOnWorldGenDone;
    private Action<Player> cbOnPlayerCreated;
    private Action<AIEntity> cbOnAIEntityCreated;

    void Start()
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        CreateMapData();
        CreatePlayer();
        CreateAIEntities();

        Random.state = oldState;

        cbOnWorldGenDone?.Invoke();

        // Destroy the world generator component
        Destroy(this);
    }

    private void CreatePlayer()
    {
        Tile playerTile = WorldData.Instance.GetTile(width/2, height/2);
        Player player = new Player(playerTile, EntityType.Player);
        playerTile.entity = player;
        cbOnPlayerCreated?.Invoke(player);
    }

    private void CreateAIEntities()
    {
        // For now generate a dog
        Tile dogTile = WorldData.Instance.GetTile(width / 2, height / 4);
        AIEntity dog = new AIEntity(dogTile, EntityType.Dog);
        dogTile.entity = dog;

        cbOnAIEntityCreated?.Invoke(dog);
    }

    private void CreateMapData()
    {
        WorldData.Instance.MapData = new Tile[width * height];
        WorldData.Instance.Width = width;
        WorldData.Instance.Height = height;

        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);
            WorldData.Instance.MapData[i] = new Tile(x, y, TileType.OpenArea);
        }

        // Provide each tile with its neighboring tiles
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            Tile[] neighbors =
                WorldData.Instance.GetNeighboringTiles(
                    WorldData.Instance.MapData[i]);
            WorldData.Instance.MapData[i].SetNeighbors(neighbors);
        }
    }

    public void RegisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldGenDone += callbackfunc;
    }

    public void UnregisterOnWorldCreated(Action callbackfunc)
    {
        cbOnWorldGenDone -= callbackfunc;
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
