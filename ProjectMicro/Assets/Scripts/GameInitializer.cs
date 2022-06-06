using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField]
    public int Seed { get; private set; } = 0;
    [SerializeField]
    private int worldWidth = 100;
    [SerializeField]
    private int worldHeight = 100;

    public static GameInitializer Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ItemDatabase.CreateDatabase();

        AreaDataManager.Instance.Initiate(worldWidth, worldHeight);

        // Check if player wanted to load game
        if (SceneBus.Instance != null)
        {
            if (SceneBus.Instance.IsLoadGame)
            {
                SaveSerial.Instance.ButtonToLoadGame();
                return;
            }
        }

        // Otherwise, normal world generation process
        Seed = Random.Range(-10000, 10000);
        WorldGenerator.Instance.StartGeneration(worldWidth, worldHeight);
    }

    public void OnDataLoaded(MapType loadedMapType, int loadedSeed)
    {
        Seed = loadedSeed;

        if (loadedMapType == MapType.World)
        {
            WorldGenerator.Instance.OnDataLoaded();
        }
        else
        {
            LocationGenerator.Instance.OnDataLoaded();
        }
    }



}
