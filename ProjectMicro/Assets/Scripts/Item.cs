using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ItemCategory { RawMaterial, FinalGoods };
[Serializable]
public class Item
{
    public string itemName;
    public int baseCost;
    public ItemCategory category;

    public Item(string itemName, int baseCost, ItemCategory category)
    {
        this.itemName = itemName;
        this.baseCost = baseCost;
        this.category = category;
    }
}
