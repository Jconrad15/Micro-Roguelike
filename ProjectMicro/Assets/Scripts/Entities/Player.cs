using System;
using System.Collections.Generic;

[Serializable]
public class Player : Entity
{
    // Contructor for new player
    public Player(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        entityName = "player";
    }

    // Constructor for loaded player
    public Player(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, Tile t = null) : base(t, type)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Money = money;
        Visibility = visibility;
    }
}
