using System;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    private Action cbOnPlayerWin;
    private Action cbOnPlayerLose;

    public bool GameIsDone { get; private set; }

    // Make singleton
    public static WinLoseManager Instance;
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

    public void Win()
    {
        GameIsDone = true;
        cbOnPlayerWin?.Invoke();
    }

    public void Lose()
    {
        GameIsDone = true;
        cbOnPlayerLose?.Invoke();
    }

    public void RegisterOnPlayerWin(Action callbackfunc)
    {
        cbOnPlayerWin += callbackfunc;
    }

    public void UnregisterOnPlayerWin(Action callbackfunc)
    {
        cbOnPlayerWin -= callbackfunc;
    }

    public void RegisterOnPlayerLose(Action callbackfunc)
    {
        cbOnPlayerLose += callbackfunc;
    }

    public void UnregisterOnPlayerLose(Action callbackfunc)
    {
        cbOnPlayerLose -= callbackfunc;
    }
}
