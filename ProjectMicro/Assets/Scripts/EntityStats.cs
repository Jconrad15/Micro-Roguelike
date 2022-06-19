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
        FavorInfluenceModifier = 0;
        FavorResistanceModifier = 0;

        InventorySize = 10;
        BecomeFollowerThreshold = 3;

        BuyingPreferedSellCostModifier = -0.2f;
        BuyingPreferedBuyCostModifier = 0.1f;

        SellingPreferedSellCostModifier = -0.2f;
        SellingPreferedBuyCostModifier = 0.1f;

        SellSameGuildCostModifier = 0.1f;

        SellSkillModifier = 0;
        BuySkillModifier = 0;

        VisibilityDistance = 8;
    }

    public float VisibilityDistance { get;private set; }
    public void AdjustVisibilityDistance(float visionDistanceDelta)
    {
        VisibilityDistance += visionDistanceDelta;
    }

    public float FavorInfluenceModifier { get; private set; }
    public void SetFavorInfluenceModifier(float value)
    {
        FavorInfluenceModifier = value;
    }

    public float FavorResistanceModifier { get; private set; }
    public void SetFavorResistanceModifier(float value)
    {
        FavorResistanceModifier = value;
    }

    public int InventorySize { get; private set; }
    public void AdjustInventorySize(int inventorySizeDelta)
    {
        InventorySize += inventorySizeDelta;
    }

    public float BecomeFollowerThreshold { get; private set; }
    public void SetBecomeFollowerThreshold(float newValue)
    {
        BecomeFollowerThreshold = newValue;
    }

    public float BuyingPreferedSellCostModifier { get; private set; }
    public void SetBuyingPreferedSellCostModifier(float newValue)
    {
        BuyingPreferedSellCostModifier = newValue;
    }

    public float BuyingPreferedBuyCostModifier { get; private set; }
    public void SetBuyingPreferedBuyCostModifier(float newValue)
    {
        BuyingPreferedBuyCostModifier = newValue;
    }

    public float SellingPreferedSellCostModifier { get; private set; }
    public void SetSellingPreferedSellCostModifier(float newValue)
    {
        SellingPreferedSellCostModifier = newValue;
    }

    public float SellingPreferedBuyCostModifier { get; private set; }
    public void SetSellingPreferedBuyCostModifier(float newValue)
    {
        SellingPreferedBuyCostModifier = newValue;
    }

    /// <summary>
    /// Positive for good at selling.
    /// </summary>
    public float SellSkillModifier { get; private set; }
    public void SetSellSkillModifier(float newValue)
    {
        SellSkillModifier = newValue;
    }

    /// <summary>
    /// Negative for good at buying
    /// </summary>
    public float BuySkillModifier { get; private set; }
    public void SetBuySkillModifier(float newValue)
    {
        BuySkillModifier = newValue;
    }

    public float SellSameGuildCostModifier { get; private set; }
    public void SetSellSameGuildCostModifier(float newValue)
    {
        SellSameGuildCostModifier = newValue;
    }

}
