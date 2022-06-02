using System;
using System.Collections.Generic;

[Serializable]
public class Player : Entity
{
    private Action<PlayerLicense> cbOnPlayerLicenseChanged;
    private Action<int> cbOnPlayerMoneyChanged;

    public enum PlayerLicense { Traveller, Merchant };
    private PlayerLicense license;
    public PlayerLicense License
    {
        get => license;
        protected set
        {
            license = value;
            cbOnPlayerLicenseChanged?.Invoke(license);
        }
    }

    private int money;
    public new int Money
    { 
        get => money;
        protected set
        {
            money = value;
            cbOnPlayerMoneyChanged?.Invoke(money);
        }
    }

    // Contructor for new player
    public Player(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "player";
        License = PlayerLicense.Traveller;
        Money = startingMoney;
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
            License = PlayerLicense.Merchant;
            return true;
        }

        return false;
    }

    public bool TryPayTribute(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            return true;
        }

        return false;
    }

    public void RegisterOnLicenseChanged(Action<PlayerLicense> callbackfunc)
    {
        cbOnPlayerLicenseChanged += callbackfunc;
    }

    public void UnregisterOnLicenseChanged(Action<PlayerLicense> callbackfunc)
    {
        cbOnPlayerLicenseChanged -= callbackfunc;
    }

    public void RegisterOnMoneyChanged(Action<int> callbackfunc)
    {
        cbOnPlayerMoneyChanged += callbackfunc;
    }

    public void UnregisterOnMoneyChanged(Action<int> callbackfunc)
    {
        cbOnPlayerMoneyChanged -= callbackfunc;
    }
}
