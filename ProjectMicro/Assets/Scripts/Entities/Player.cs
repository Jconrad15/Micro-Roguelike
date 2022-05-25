using System;
using System.Collections.Generic;

[Serializable]
public class Player : Entity
{
    // Contructor for new player
    public Player(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "player";
    }

    // Constructor for loaded player
    public Player(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, Tile t = null) : base(t, type, money)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Visibility = visibility;
    }
}
