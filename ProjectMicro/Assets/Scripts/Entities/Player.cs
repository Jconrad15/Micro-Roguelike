using System;
using System.Collections.Generic;

[Serializable]
public class Player : Entity
{
    private Action<int> cbOnFoodLevelChanged;

    private int foodLevel;
    public int FoodLevel 
    {
        get
        {
            return foodLevel;
        }
        protected set
        {
            foodLevel = value;
            cbOnFoodLevelChanged?.Invoke(foodLevel);
        }
    }

    public int FoodLevelMax { get; protected set; } = 10;

    public enum PlayerTitle { Traveller, Merchant };
    public PlayerTitle Title { get; protected set; }

    // Contructor for new player
    public Player(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "player";
        Title = PlayerTitle.Traveller;
        foodLevel = FoodLevelMax;
    }

    // Constructor for loaded player
    public Player(EntityType type, List<Item> inventoryItems,
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

    public bool TryPurchaseTitle(int cost)
    {
        if (Money >= cost)
        {
            Money -= cost;
            Title = PlayerTitle.Merchant;
            return true;
        }

        return false;
    }

    public bool TryEatFood(Item foodItem)
    {
        if (foodItem == null) { return false; }
        if (InventoryItems.Contains(foodItem) == false) { return false; }

        // If food level can increase
        if (FoodLevel < FoodLevelMax)
        {
            // Remove food item from inventory and satisfy player hunger 
            _ = InventoryItems.Remove(foodItem);
            IncreaseFoodLevel();
            return true;
        }

        return false;
    }

    private void IncreaseFoodLevel()
    {
        FoodLevel += 1;
    }

    private void DecreaseFoodLevel()
    {
        FoodLevel -= 1;
    }

    public void RegisterOnFoodLevelChanged(Action<int> callbackfunc)
    {
        cbOnFoodLevelChanged += callbackfunc;
    }

    public void UnregisterOnFoodLevelChanged(Action<int> callbackfunc)
    {
        cbOnFoodLevelChanged -= callbackfunc;
    }
}
