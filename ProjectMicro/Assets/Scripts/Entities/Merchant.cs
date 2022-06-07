using System.Collections.Generic;
using UnityEngine;

// TODO: Future genereal ideas:
// Fisher, Botanist, Armorer, Carpenter,
// LeatherWorker, Weaver, Goldsmith, Cook, Alchemist
/*Enchanter, Butcher, Bookstore, Tailor, Barber, Fletcher,
Cooper, Farrier, Carpenter, Candlemaker Baker Weaver
Leatherworker Cobbler Tanner Apothecary Alchemist 
Clothing Brewery Glassblower Pottery Porcelain Florist 
Tinker Thatcher Messenger service bookstore Weaponsmith Food Fish Market
Fresh Produce Cartographer Stables University Hatter Bank Spice 
Shipwrights Wineries*/

public enum MerchantType {
    WoodCutter, Miner, Blacksmith, Traveller, Potter, Baker, Farmer };
public class Merchant : AIEntity
{
    private const float sellModifier = 0.2f;
    protected int waitAtTileTurns;
    public MerchantType MType { get; protected set; }
    protected MerchantTypeRef typeRef;

    public Merchant(
        Tile t, EntityType type,
        MerchantType merchantType, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "merchant";
        MType = merchantType;
        LoadMerchantTypeRef();
        CreateMerchantStartingInventory();
    }

    // Constructor for loaded Merchant
    public Merchant(EntityType type, List<Item> inventoryItems,
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

    public int GetAdjustedCost(Item itemInQuestion)
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

        // If the merchant prefers to sell the item 
        // adjust the cost down, since this is what the merchant always sells
        // e.g., a woodcutter always has wood and sells it for less
        if (typeRef.preferredSell != null)
        {
            for (int i = 0; i < typeRef.preferredSell.Length; i++)
            {
                if (itemInQuestion.itemName ==
                    typeRef.preferredSell[i].itemName)
                {
                    modifier -= sellModifier;
                    break;
                }
            }
        }

        // If the merchant prefers to buy the item
        // adjust the cost up, since this is what the mechant wants
        // e.g., a woodcutter needs saws, and buys them for more. 
        if (typeRef.preferredBuy != null)
        {
            for (int i = 0; i < typeRef.preferredBuy.Length; i++)
            {
                if (itemInQuestion.itemName ==
                    typeRef.preferredBuy[i].itemName)
                {
                    modifier += sellModifier;
                    break;
                }
            }
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
