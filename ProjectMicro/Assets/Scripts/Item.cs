using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
    public float baseCost;

    public Item(string name, float baseCost)
    {
        this.name = name;
        this.baseCost = baseCost;
    }
}
