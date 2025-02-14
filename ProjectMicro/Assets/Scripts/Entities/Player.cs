using System;
using System.Collections.Generic;

[Serializable]
public class Player : Entity
{
    private FollowerManager followerManager;

    private Action<PlayerLicense> cbOnPlayerLicenseChanged;
    private Action<Follower> cbOnFollowerAdded;

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

    /// <summary>
    /// Constructor for player created in game setup.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="type"></param>
    /// <param name="startingMoney"></param>
    /// <param name="p"></param>
    public Player(
        Tile t, EntityType type, int startingMoney, 
        Player p, List<Trait> traits)
    : base(t, type, startingMoney, traits)
    {
        EntityName = p.EntityName;
        License = PlayerLicense.Traveller;
        Money = p.Money;
        InventoryItems = p.InventoryItems;
        CharacterName = p.CharacterName;
        CurrentGuild = p.CurrentGuild;

        followerManager = new FollowerManager();
    }

    /// <summary>
    /// Constructor for loaded player.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inventoryItems"></param>
    /// <param name="money"></param>
    /// <param name="visibility"></param>
    /// <param name="entityName"></param>
    /// <param name="characterName"></param>
    /// <param name="t"></param>
    public Player(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, Guild guild, List<Trait> traits,
        Tile t = null)
        : base(t, type, money, traits)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Visibility = visibility;
        EntityName = entityName;
        CharacterName = characterName;
        Money = money;

        if (t != null)
        {
            T = t;
        }
        CurrentGuild = guild;
        followerManager = new FollowerManager();

        Traits = traits;
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

    public void TryAddFollower(Entity entity)
    {
        // Check entity favor
        if (entity.Favor < entity.stats.BecomeFollowerThreshold) { return; }

        // Add follower
        Follower f = followerManager.AddFollower(entity);

        // Edit player to be better, due to follower
        // TODO: better inventory size change
        stats.AdjustInventorySize(2);

        cbOnFollowerAdded?.Invoke(f);

        // Remove entity from the world
        AIEntity aiEntity = (AIEntity)entity;
        aiEntity.RemoveFromAreaToBeFollower();
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

    public void RegisterOnFollowerAdded(Action<Follower> callbackfunc)
    {
        cbOnFollowerAdded += callbackfunc;
    }

    public void UnregisterOnFollowerAdded(Action<Follower> callbackfunc)
    {
        cbOnFollowerAdded -= callbackfunc;
    }
}
