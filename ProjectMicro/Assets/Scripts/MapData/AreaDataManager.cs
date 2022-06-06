using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDataManager : MonoBehaviour
{

    public static AreaDataManager Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public AreaData CurrentLocationData;
    public AreaData WorldData;

    public AreaData[] AllLocationData;

    public void Initiate(int worldWidth, int worldHeight)
    {
        CurrentLocationData = new AreaData();
        WorldData = new AreaData();

        AllLocationData = new AreaData[worldWidth * worldHeight];
    }
}
