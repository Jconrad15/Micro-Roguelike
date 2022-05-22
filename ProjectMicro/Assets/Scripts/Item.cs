using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory { RawMaterial };
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
