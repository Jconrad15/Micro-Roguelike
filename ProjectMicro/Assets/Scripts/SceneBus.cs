using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBus : MonoBehaviour
{
    public bool IsLoadGame { get; private set; }
    public static SceneBus Instance { get; private set; }
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

    public void SetIsLoadGame(bool isLoadGame) => IsLoadGame = isLoadGame;
}
