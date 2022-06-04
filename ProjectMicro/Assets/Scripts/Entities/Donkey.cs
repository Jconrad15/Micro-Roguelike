using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : Animal
{
    public Donkey(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "donkey";
    }

    // Constructor for loaded Donkey
    public Donkey(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, Tile t = null) : base(t, type, money)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Visibility = visibility;
        EntityName = entityName;
        CharacterName = characterName;

        if (t != null)
        {
            T = t;
        }
    }

}
