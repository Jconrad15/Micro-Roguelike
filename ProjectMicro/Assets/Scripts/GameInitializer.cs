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

    public GuildManager CurrentGuildManager { get; private set; }

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
                FindObjectOfType<GameSetupUI>().Hide();
                return;
            }
        }
        FindObjectOfType<GameSetupUI>().Show();

    }

    public void InitializeGame()
    {
        // Otherwise, normal world generation process
        Seed = Random.Range(-10000, 10000);
        
        CurrentGuildManager = new GuildManager(Seed);
        
        WorldGenerator.Instance.StartGeneration(worldWidth, worldHeight);
    }

    public void LoadGuildManager(int loadedSeed)
    {
        Seed = loadedSeed;
        CurrentGuildManager = new GuildManager(Seed);
    }

    public void OnDataLoaded(MapType loadedMapType)
    {
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
