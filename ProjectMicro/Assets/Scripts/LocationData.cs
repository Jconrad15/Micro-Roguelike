using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton holding location data
/// </summary>
public class LocationData : AreaData
{
    public static LocationData Instance { get; private set; }
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
}
