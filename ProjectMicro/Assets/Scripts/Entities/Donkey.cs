using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Donkey : Animal
{
    public Donkey(
        Tile t, EntityType type, int startingMoney,
        List<Attribute> attributes)
        : base(t, type, startingMoney, attributes)
    {
        EntityName = "donkey";
    }

    // Constructor for loaded Donkey
    public Donkey(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, List<Attribute> attributes, 
        Tile t = null)
        : base(t, type, money, attributes)
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
