using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : AIEntity
{
    protected int waitAtTileTurns;

    public Merchant(Tile t, EntityType type) : base(t, type) 
    {
        entityName = "merchant";
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
