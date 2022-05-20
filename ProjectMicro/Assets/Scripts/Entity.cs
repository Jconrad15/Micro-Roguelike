using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Player, Dog };
public class Entity
{
    public int X { get; protected set; }
    public int Y { get; protected set; }

    // Updating T will update X and Y coordinates
    private Tile t;
    public Tile T 
    {
        get 
        {
            return t;
        }
        protected set
        {
            t = value;
            X = value.x;
            Y = value.y;
        }
    }

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

    public Entity(Tile t, EntityType type)
    {
        T = t;
        this.type = type;
        X = t.x;
        Y = t.y;
        Visibility = VisibilityLevel.NotVisible;

        // Add self to entity list
        WorldData.Instance.AddEntity(this);
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
}
