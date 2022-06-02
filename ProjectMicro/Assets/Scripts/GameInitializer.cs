using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
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

    void Start()
    {
        // Check if player wanted to load game
        if (SceneBus.Instance != null)
        {
            if (SceneBus.Instance.IsLoadGame)
            {
                SaveSerial.Instance.ButtonToLoadGame();
                return;
            }
        }

        // Normal world generation process
        WorldGenerator.Instance.StartGeneration();
    }
}
