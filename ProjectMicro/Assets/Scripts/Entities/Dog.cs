using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Animal
{

    public Dog(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "dog";
    }

    // Constructor for loaded Dog
    public Dog(EntityType type, List<Item> inventoryItems,
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
