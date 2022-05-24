using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Future genereal ideas:
// Fisher, Miner, Botanist, Armorer, Carpenter, Blacksmith,
// LeatherWorker, Weaver, Goldsmith, Cook, Alchemist
/*Blacksmith shop, Enchanter's shop, Butcher, Bookstore, Tailor, Barber, Fletcher,
Cooper, Farrier, Carpenter, Candlemaker Baker Haberdashery Fabric shop Weaver
Leatherworker Cobbler Tanner Apothecary Alchemist supply store Flower boutique 
Clothing boutique Saloon Potion Brewery Clockwork repair Tea House Glassblower
Pottery shop Porcelain shop Florist Tinker Thatcher Messenger service 
Magic bookstore Inn Tavern Weaponsmith Armorer Food carts Grocery store Fish Market
Fresh Produce Cartographer Stables University Hatter Bank Spice shop Lumber yard
Brewery Adventuring Supply Store Art Emporium Dye Maker Shipwrights Wineries*/

public enum MerchantType { WoodCutter };
public class Merchant : AIEntity
{
    private const float sellModifier = 0.2f;
    protected int waitAtTileTurns;
    protected MerchantType merchantType;
    protected MerchantTypeRef typeRef;

    public Merchant(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        entityName = "merchant";
        LoadMerchantTypeRef();
        CreateMerchantStartingInventory();
    }

    private void LoadMerchantTypeRef()
    {
        if (MerchantTypeDatabase.database == null)
        {
            MerchantTypeDatabase.CreateDatabase();
        }

        typeRef = MerchantTypeDatabase.GetMerchantTypeRef(merchantType);
    }

    private void CreateMerchantStartingInventory()
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

    private int GetAdjustedCost(Item itemInQuestion)
    {
        if (itemInQuestion == null) { return int.MaxValue; }

        float modifier = 1f;

        // If the merchant prefers to sell the item 
        // adjust the cost down, since this is what the merchant always sells
        // e.g., a woodcutter always has wood and sells it for less
        if (typeRef.preferredSell != null)
        {
            for (int i = 0; i < typeRef.preferredSell.Length; i++)
            {
                if (itemInQuestion.itemName == typeRef.preferredSell[i].itemName)
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
                if (itemInQuestion.itemName == typeRef.preferredBuy[i].itemName)
                {
                    modifier += sellModifier;
                    break;
                }
            }
        }

        // Return the base cost mutlipled by any modifier, but truncated to int
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
