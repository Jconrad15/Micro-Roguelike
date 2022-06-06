using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Player, AI };

[Serializable]
public class SerializableEntity
{
    public int x;
    public int y;
    public string entityName;
    public string characterName;
    public List<Item> inventoryItems;
    public int money;
    public EntityType type;
    public VisibilityLevel visibility;
}

public class Entity
{
    /// <summary>
    /// Helps indicate the type of entity
    /// </summary>
    public string EntityName { get; protected set; }
    public string CharacterName { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    // Updating T will update X and Y coordinates
    private Tile t;
    public Tile T
    {
        get => t;
        protected set
        {
            t = value;
            if (value != null)
            {
                X = value.x;
                Y = value.y;
            }
        }
    }

    public List<Item> InventoryItems { get; protected set; }
    public int InventorySize { get; protected set; } = 10;

    protected Action<int> cbOnPlayerMoneyChanged;
    private int money;
    public int Money 
    { 
        get => money;
        protected set
        {
            int delta = value - money;
            if (delta != 0)
            {
                cbOnMoneyDelta?.Invoke(delta);
            }

            money = value;
            cbOnPlayerMoneyChanged?.Invoke(money);
        }
    }

    public EntityType type;
    protected VisibilityLevel visibility;
    public VisibilityLevel Visibility
    {
        get { return visibility; }
        set
        {
            visibility = value;
            cbOnVisibilityChanged?.Invoke(this);
        }
    }

    protected Action<Entity> cbOnVisibilityChanged;
    protected Action<Entity, Vector2> cbOnMove;
    protected Action<Entity> cbOnMerchantClicked;
    protected Action<Entity> cbOnPlayerClicked;
    protected Action<int> cbOnMoneyDelta;
    protected Action<Item> cbOnItemPurchased;
    protected Action<Item> cbOnItemSold;

    public int TurnsNotMovedStuck { get; protected set; } = 0;

    public void SetTile(Tile tile)
    {
        if (tile == null) { return; }
        T = tile;
    }

    /// <summary>
    /// Constructor to create new entity.
    /// </summary>
    /// <param name="t"></param>
    /// <param name="type"></param>
    /// <param name="startingMoney"></param>
    public Entity(Tile t, EntityType type, int startingMoney)
    {
        if (t == null) { return; }
        T = t;
        X = t.x;
        Y = t.y;
        Money = startingMoney;

        this.type = type;

        Visibility = VisibilityLevel.NotVisible;

        InventoryItems = new List<Item>();

        CreateCharacterName();

        // Add self to entity list
        AreaData areaData = AreaData.GetAreaDataForCurrentType();
        areaData.AddEntity(this);
    }

    public void PlayerClickOnPlayer()
    {
        cbOnPlayerClicked?.Invoke(this);
    }

    public bool TryMove(Direction d)
    {
        // Get tile in movement direction
        Tile neighbor = T.neighbors[(int)d];

        // No movement if no neighbor tile
        if (neighbor == null) 
        {
            TurnsNotMovedStuck++;
            return false; 
        }

        // No movement if the tile is not walkable
        if (neighbor.isWalkable == false) 
        {
            TurnsNotMovedStuck++;
            return false;
        }

        // No movement if there is entity in neighbor tile
        if (neighbor.entity != null)
        {
            TurnsNotMovedStuck++;
            return false;
        }

        Move(neighbor);
        return true;
    }

    public bool TryMove(Tile destTile)
    {
        // No movement if no neighbor tile
        if (destTile == null)
        {
            TurnsNotMovedStuck++;
            return false;
        }

        // No movement if there is entity in neighbor tile
        if (destTile.entity != null)
        {
            TurnsNotMovedStuck++;
            return false;
        }

        // No movement if the tile is not walkable
        if (destTile.isWalkable == false)
        {
            TurnsNotMovedStuck++;
            return false;
        }

        Move(destTile);
        return true;
    }

    protected void Move(Tile destination)
    {
        Vector2 startPos = new Vector2 (X, Y);

        // Remove self from current tile
        T.entity = null;

        // Move self to neighbor tile
        destination.entity = this;

        // Set as new tile
        T = destination;

        cbOnMove?.Invoke(this, startPos);
    }

    /// <summary>
    /// Triggered when a clicked on trader is transacted with.
    /// </summary>
    /// <param name="itemToTransfer"></param>
    /// <param name="player"></param>
    /// <param name="clickedEntity"></param>
    /// <param name="isPlayerItem"></param>
    /// <returns></returns>
    public bool TryTransferItem(Item itemToTransfer, Player player,
        Entity clickedEntity, bool isPlayerItem)
    {
        Merchant m = (Merchant)clickedEntity;
        int adjustedItemCost = m.GetAdjustedCost(itemToTransfer);

        // Transfer to the trader if possible
        if (isPlayerItem)
        {
            // If the trader has enough money and inventory space
            if (adjustedItemCost <= m.Money &&
                InventoryItems.Count < InventorySize)
            {
                m.AddPurchasedItem(itemToTransfer, adjustedItemCost);
                player.RemoveSoldItem(itemToTransfer, adjustedItemCost);
                return true;
            }
            // The trader does not have enough money or enough space
            else
            {
                return false;
            }
        }
        // Trasfer to the player if possible
        else
        {
            // If the player has enough money and inventory space
            if (adjustedItemCost <= player.Money &&
                player.InventoryItems.Count < InventorySize)
            {
                player.AddPurchasedItem(itemToTransfer, adjustedItemCost);
                m.RemoveSoldItem(itemToTransfer, adjustedItemCost);
                return true;
            }
            // The player does not have enough money or space
            else
            {
                Debug.Log("no money or no space");
                return false;
            }
        }
    }

    /// <summary>
    /// Adds the purchased item to the inventory list and removes money.
    /// </summary>
    /// <param name="item"></param>
    public void AddPurchasedItem(Item item, int adjustedCost)
    {
        Money -= adjustedCost;
        InventoryItems.Add(item);
        cbOnItemPurchased?.Invoke(item);
    }

    /// <summary>
    /// Removes the sold item from the inventory list and adds money.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveSoldItem(Item item, int adjustedCost)
    {
        Money += adjustedCost;
        _ = InventoryItems.Remove(item);
        cbOnItemSold?.Invoke(item);
    }

    protected void CreateCharacterName()
    {
        CharacterName = NameGenerator.GenerateName();
    }

    public virtual void ClearData()
    {
        InventoryItems = null;

        if (T != null)
        {
            T.entity = null;
        }

        T = null;
        EntityName = null;
        CharacterName = null;
        cbOnMerchantClicked = null;
        cbOnMove = null;
        cbOnPlayerClicked = null;
        cbOnPlayerMoneyChanged = null;
        cbOnVisibilityChanged = null;
    }

    public void RegisterOnMove(Action<Entity, Vector2> callbackfunc)
    {
        cbOnMove += callbackfunc;
    }

    public void UnregisterOnMove(Action<Entity, Vector2> callbackfunc)
    {
        cbOnMove -= callbackfunc;
    }

    public void RegisterOnVisibilityChanged(Action<Entity> callbackfunc)
    {
        cbOnVisibilityChanged += callbackfunc;
    }

    public void UnregisterOnVisibilityChanged(Action<Entity> callbackfunc)
    {
        cbOnVisibilityChanged -= callbackfunc;
    }

    public void RegisterOnMerchantClicked(Action<Entity> callbackfunc)
    {
        cbOnMerchantClicked += callbackfunc;
    }

    public void UnregisterOnMerchantClicked(Action<Entity> callbackfunc)
    {
        cbOnMerchantClicked -= callbackfunc;
    }

    public void RegisterOnPlayerClicked(Action<Entity> callbackfunc)
    {
        cbOnPlayerClicked += callbackfunc;
    }

    public void UnregisterOnPlayerClicked(Action<Entity> callbackfunc)
    {
        cbOnPlayerClicked -= callbackfunc;
    }

    public void RegisterOnMoneyDelta(Action<int> callbackfunc)
    {
        cbOnMoneyDelta += callbackfunc;
    }

    public void UnregisterOnMoneyDelta(Action<int> callbackfunc)
    {
        cbOnMoneyDelta -= callbackfunc;
    }

    public void RegisterOnItemPurchased(Action<Item> callbackfunc)
    {
        cbOnItemPurchased += callbackfunc;
    }

    public void UnregisterOnItemPurchased(Action<Item> callbackfunc)
    {
        cbOnItemPurchased -= callbackfunc;
    }

    public void RegisterOnItemSold(Action<Item> callbackfunc)
    {
        cbOnItemSold += callbackfunc;
    }

    public void UnregisterOnItemSold(Action<Item> callbackfunc)
    {
        cbOnItemSold -= callbackfunc;
    }
}
