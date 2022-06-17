using System;
using UnityEngine;

[Serializable]
public class EntityStats
{
    /// <summary>
    /// Base value constructor.
    /// </summary>
    public EntityStats()
    {
        FavorGainRate = 1;
        InventorySize = 10;
        BecomeFollowerThreshold = 3;
    }

    public float FavorGainRate { get; private set; }
    public void SetFavorGainRate(float fgr) => FavorGainRate = fgr;

    public int InventorySize { get; private set; }
    public void AdjustInventorySize(int inventorySizeDelta)
    {
        InventorySize += inventorySizeDelta;
    }

    public float BecomeFollowerThreshold { get; private set; }
    public void SetBecomeFollowerThreshold(float bft)
    {
        BecomeFollowerThreshold = bft;
    }
}
