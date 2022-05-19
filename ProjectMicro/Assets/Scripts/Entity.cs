using System;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType { Player };
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

    private Action<Entity> cbOnMove;

    public Entity(Tile t, EntityType type)
    {
        T = t;
        this.type = type;
        X = t.x;
        Y = t.y;
        Debug.Log(X + " " + Y);
    }

    public bool TryMove(Direction d)
    {
        // Get tile in movement direction
        Tile neighbor = T.neighbors[(int)d];

        // No movement if no neighbor tile
        if (neighbor == null) { return false; }

        // No movement if there is entity in neighbor tile
        if (neighbor.entity != null) { return false; }

        Move(neighbor);
        return true;
    }

    private void Move(Tile destination)
    {
        // Remove self from current tile
        T.entity = null;

        // Move self to neighbor tile
        destination.entity = this;

        // Set as new tile
        T = destination;

        cbOnMove?.Invoke(this);
    }

    public void RegisterOnMove(Action<Entity> callbackfunc)
    {
        cbOnMove += callbackfunc;
    }

    public void UnregisterOnMove(Action<Entity> callbackfunc)
    {
        cbOnMove -= callbackfunc;
    }
}
