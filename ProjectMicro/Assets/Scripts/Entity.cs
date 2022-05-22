using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Player, Trader, Dog };
public class Entity
{
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
            X = value.x;
            Y = value.y;
        }
    }

    public List<Item> InventoryItems { get; protected set; }

    // TODO: for now just have everyone start with 10 money
    public int Money { get; protected set; } = 10;

    public EntityType type;
    private VisibilityLevel visibility;
    public VisibilityLevel Visibility
    {
        get { return visibility; }
        set
        {
            visibility = value;
            cbOnVisibilityChanged?.Invoke(this);
        }
    }

    private Action<Entity> cbOnVisibilityChanged;
    private Action<Entity, Vector2> cbOnMove;
    private Action<Entity> cbOnTraderClicked;

    public Entity(Tile t, EntityType type)
    {
        T = t;
        this.type = type;
        X = t.x;
        Y = t.y;
        Visibility = VisibilityLevel.NotVisible;

        InventoryItems = new List<Item>();

        // Add self to entity list
        WorldData.Instance.AddEntity(this);
    }

    public void PlayerClickOnTrader()
    {
        cbOnTraderClicked?.Invoke(this);
    }

    public bool TryMove(Direction d)
    {
        // Get tile in movement direction
        Tile neighbor = T.neighbors[(int)d];

        // No movement if no neighbor tile
        if (neighbor == null) { return false; }

        // No movement if the tile is not walkable
        if (neighbor.isWalkable == false) { return false; }

        // No movement if there is entity in neighbor tile
        if (neighbor.entity != null) { return false; }

        Move(neighbor);
        return true;
    }

    public bool TryMove(Tile destTile)
    {
        // No movement if no neighbor tile
        if (destTile == null) { return false; }

        // No movement if the tile is not walkable
        if (destTile.isWalkable == false) { return false; }

        // No movement if there is entity in neighbor tile
        if (destTile.entity != null) { return false; }

        Move(destTile);
        return true;
    }

    private void Move(Tile destination)
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

        Debug.Log("Item: " + item.name + " transfered to " + type.ToString());
    }

    /// <summary>
    /// Removes the sold item from the inventory list and adds money.
    /// </summary>
    /// <param name="item"></param>
    public void RemoveSoldItem(Item item)
    {
        Money += item.baseCost;
        _ = InventoryItems.Remove(item);

        Debug.Log("Item: " + item.name + " removed from " + type.ToString());
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

    public void RegisterOnTraderClicked(Action<Entity> callbackfunc)
    {
        cbOnTraderClicked += callbackfunc;
    }

    public void UnregisterOnTraderClicked(Action<Entity> callbackfunc)
    {
        cbOnTraderClicked -= callbackfunc;
    }
}
