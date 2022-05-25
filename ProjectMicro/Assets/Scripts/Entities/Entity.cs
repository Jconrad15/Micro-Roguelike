using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Player, AI };

[Serializable]
public class SerializableEntity
{
    public int x;
    public int y;
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

    // TODO: for now just have everyone start with 10 money
    public int Money { get; protected set; }

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
        // Transfer to the trader if possible
        if (isPlayerItem)
        {
            // If the trader has enough money
            if (itemToTransfer.baseCost <= clickedEntity.Money)
            {
                clickedEntity.AddPurchasedItem(itemToTransfer);
                player.RemoveSoldItem(itemToTransfer);
                return true;
            }
            // The trader does not have enough money
            else
            {
                return false;
            }
        }
        // Trasfer to the player if possible
        else
        {
            // If the player has enough money
            if (itemToTransfer.baseCost <= player.Money)
            {
                player.AddPurchasedItem(itemToTransfer);
                clickedEntity.RemoveSoldItem(itemToTransfer);
                return true;
            }
            // The trader does not have enough money
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Adds the purchased item to the inventory list and removes money.
    /// </summary>
    /// <param name="item"></param>
    public void AddPurchasedItem(Item item)
    {
        Money -= item.baseCost;
        InventoryItems.Add(item);
    }

    /// <summary>
    /// Removes the sold item from the inventory list and adds money.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveSoldItem(Item item)
    {
        Money += item.baseCost;
        _ = InventoryItems.Remove(item);
    }

    protected void CreateCharacterName()
    {
        CharacterName = "Character name!";
    }

    public void Destroy()
    {
        InventoryItems.Clear();
        T = null;
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
}
