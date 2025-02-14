using System.Collections.Generic;
using UnityEngine;

// TODO: Future genereal ideas:
/*Botanist, Armorer LeatherWorker, Weaver, Goldsmith, Alchemist
Enchanter, Butcher, Tailor, Barber, Fletcher,
Cooper, Farrier, Candlemaker, Weaver
Cobbler Apothecary Brewery Glassblower Porcelain Florist 
Tinker Thatcher Cartographer Stables University Hatter Bank Spice 
Shipwrights*/

public enum MerchantType 
{
    WoodCutter, Miner, Blacksmith, Traveller, Potter, Baker, Farmer,
    Fisher, Carpenter, Papersmith, Bookmaker, Vinter, Weaponsmith,
    Rancher, Tanner
};

public class Merchant : AIEntity
{
    protected int waitAtTileTurns;
    public MerchantType MType { get; protected set; }
    private MerchantTypeRef typeRef;

    public Merchant(
        Tile t, EntityType type,
        MerchantType merchantType, int startingMoney,
        List<Trait> traits)
        : base(t, type, startingMoney, traits)
    {
        EntityName = "merchant";
        MType = merchantType;
        LoadMerchantTypeRef();
        CreateMerchantStartingInventory();
    }

    /// <summary>
    /// Constructor for loaded Merchant.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="inventoryItems"></param>
    /// <param name="money"></param>
    /// <param name="visibility"></param>
    /// <param name="entityName"></param>
    /// <param name="characterName"></param>
    /// <param name="t"></param>
    public Merchant(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, Guild guild, float favor, List<Trait> traits,
        EntityStats stats, Tile t = null)
        : base(t, type, money, traits)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Visibility = visibility;
        EntityName = entityName;
        CharacterName = characterName;
        CurrentGuild = guild;
        Favor = favor;
        this.stats = stats;

        if (t != null)
        {
            T = t;
        }

        LoadMerchantTypeRef();
    }

    private void LoadMerchantTypeRef()
    {
        if (MerchantTypeDatabase.database == null)
        {
            MerchantTypeDatabase.CreateDatabase();
        }

        typeRef = MerchantTypeDatabase.GetMerchantTypeRef(MType);
    }

    private void CreateMerchantStartingInventory()
    {
        // Travelling merchants get more random items
        if (MType == MerchantType.Traveller)
        {
            int itemCount = Random.Range(2, 5);
            for (int i = 0; i < itemCount; i++)
            {
                InventoryItems.Add(ItemDatabase.GetRandomItem());
            }
        }
        else
        {
            // Get one of each of preferred sell and preferred buy
            List<string> potentialItemNames = new List<string>();
            for (int i = 0; i < typeRef.preferredSell.Length; i++)
            {
                potentialItemNames.Add(typeRef.preferredSell[i].itemName);
            }
            for (int i = 0; i < typeRef.preferredBuy.Length; i++)
            {
                potentialItemNames.Add(typeRef.preferredBuy[i].itemName);
            }

            for (int i = 0; i < potentialItemNames.Count; i++)
            {
                InventoryItems.Add(
                    ItemDatabase.GetItemByName(potentialItemNames[i]));
            }

            // Also, get one random item
            InventoryItems.Add(ItemDatabase.GetRandomItem());
        }
    }

    public void IsPreferredBuySell(Item itemInQuestion,
        out bool isPreferredBuy, out bool isPreferredSell)
    {
        isPreferredBuy = isPreferredSell = false;

        // Check if is preferred buy
        isPreferredSell = IsPreferredSell(itemInQuestion, isPreferredSell);

        // Check if is preferred sell
        isPreferredBuy = IsPreferredBuy(itemInQuestion, isPreferredBuy);
    }

    private bool IsPreferredSell(Item itemInQuestion, bool isPreferredSell)
    {
        if (typeRef.preferredSell != null)
        {
            for (int i = 0; i < typeRef.preferredSell.Length; i++)
            {
                if (itemInQuestion.itemName ==
                    typeRef.preferredSell[i].itemName)
                {
                    isPreferredSell = true;
                }
            }
        }

        return isPreferredSell;
    }

    private bool IsPreferredBuy(Item itemInQuestion, bool isPreferredBuy)
    {
        if (typeRef.preferredBuy != null)
        {
            for (int i = 0; i < typeRef.preferredBuy.Length; i++)
            {
                if (itemInQuestion.itemName ==
                    typeRef.preferredBuy[i].itemName)
                {
                    isPreferredBuy = true;
                }
            }
        }

        return isPreferredBuy;
    }

    public int GetAdjustedCost(
        Item itemInQuestion, Player player, bool isPlayerItem)
    {
        if (itemInQuestion == null)
        {
            Debug.LogError("Trying to get adjusted cost of null item");
            return int.MaxValue;
        }
        if (typeRef == null)
        {
            Debug.LogError("No merchant typeRef.");
            return int.MaxValue; 
        }
        float modifier = 1f;

        IsPreferredBuySell(itemInQuestion,
            out bool isPreferredBuy, out bool isPreferredSell);


        if (isPlayerItem)
        {
            // This is the player's item
            if (isPreferredSell)
            {
                // If the merchant prefers to sell the item 
                // adjust the cost down, since this is what the merchant always sells
                // e.g., a woodcutter always has wood and sells it for less
                modifier += stats.BuyingPreferedSellCostModifier;
            }

            if (isPreferredBuy)
            {
                // If the merchant prefers to buy the item
                // adjust the cost up, since this is what the mechant wants
                // e.g., a woodcutter needs saws, and buys them for more. 
                modifier += stats.BuyingPreferedBuyCostModifier;
            }

            // Modify based on skill
            modifier += stats.BuySkillModifier;
            modifier += player.stats.SellSkillModifier;
        }
        else
        {
            // This is the merchant's item
            if (isPreferredSell)
            {
                // If the merchant prefers to sell the item 
                // adjust the cost down, since this is what the merchant always sells
                // e.g., a woodcutter always has wood and sells it for less
                modifier += stats.SellingPreferedSellCostModifier;
            }

            if (isPreferredBuy)
            {
                // If the merchant prefers to buy the item
                // adjust the cost up, since this is what the mechant wants
                // e.g., a woodcutter needs saws, and buys them for more. 
                modifier += stats.SellingPreferedBuyCostModifier;
            }

            // Modify based on skill
            modifier += player.stats.BuySkillModifier;
            modifier += stats.SellSkillModifier;
        }

        // Adjust the modifier based on whether or not the merchant
        // is in the same guild as the player
        if (player.CurrentGuild == CurrentGuild)
        {
            modifier -= stats.SellSameGuildCostModifier;
        }

        // Return the base cost mutlipled by any modifier,
        // but truncated to int
        return (int)(itemInQuestion.baseCost * modifier);
    }

    public void PlayerClickOnMerchant()
    {
        cbOnMerchantClicked?.Invoke(this);
    }

    protected override bool TryDetermineNewDestinationBreak()
    {
        // Merchant entity determines to wait at tile before moving
        if (waitAtTileTurns > 0)
        {
            waitAtTileTurns--;
            return false;
        }

        waitAtTileTurns = 0;
        return true;
    }

    protected override void DetermineNewDestination()
    {
        base.DetermineNewDestination();
        // Determine wait at tile length
        waitAtTileTurns = Random.Range(4, 6);
    }
}
