using System;
using System.Collections.Generic;
using UnityEngine;

public enum Season { Spring, Summer, Autumn, Winter };
/// <summary>
/// Singleton for tracking days and seasons
/// </summary>
public class SeasonManager : MonoBehaviour
{
    private Action<Season> cbOnSeasonChanged;
    private Action<int> cbOnDayChanged;

    private PlayerController playerController;

    private readonly int daysPerMonth = 30;

    // Make singleton
    public static SeasonManager Instance;
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

    private Season currentSeason;
    public Season CurrentSeason
    {
        get => currentSeason;
        private set
        {
            currentSeason = value;
            cbOnSeasonChanged?.Invoke(currentSeason);
        }
    }

    private int day;
    public int Day
    {
        get => day;
        private set
        {
            if (value > daysPerMonth)
            {
                day = 1;
                IncrementSeason();
            }
            else
            {
                day = value;
            }
            cbOnDayChanged?.Invoke(day);
        }
    }

    private void IncrementSeason()
    {
        if (CurrentSeason == Season.Winter)
        {
            CurrentSeason = Season.Spring;
        }
        else
        {
            CurrentSeason += 1;
        }
    }

    void Start()
    {
        CurrentSeason = Season.Spring;
        Day = 1;

        playerController = FindObjectOfType<PlayerController>();
        playerController.RegisterOnPlayerMove(OnPlayerMoved);
    }

    private void OnPlayerMoved(int x, int y)
    {
        // Only increment days when on world map
        if (AreaDataManager.Instance.CurrentMapType != MapType.World) { return; }

        Day += 1;
    }

    public void RegisterOnDayChanged(Action<int> callbackfunc)
    {
        cbOnDayChanged += callbackfunc;
    }

    public void UnregisterOnDayChanged(Action<int> callbackfunc)
    {
        cbOnDayChanged -= callbackfunc;
    }

    public void RegisterOnSeasonChanged(Action<Season> callbackfunc)
    {
        cbOnSeasonChanged += callbackfunc;
    }

    public void UnregisterOnSeasonChanged(Action<Season> callbackfunc)
    {
        cbOnSeasonChanged -= callbackfunc;
    }
}
