using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : Animal
{
    public Dog(
        Tile t, EntityType type, int startingMoney,
        List<Trait> traits)
        : base(t, type, startingMoney, traits)
    {
        EntityName = "dog";
    }

    // Constructor for loaded Dog
    public Dog(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, List<Trait> traits,
        Tile t = null)
        : base(t, type, money, traits)
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
