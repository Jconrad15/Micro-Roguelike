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
        if (waitAtTileTurns > 0)
        {
            waitAtTileTurns--;
            Debug.Log("Merchant wait");
            return false;
        }

        waitAtTileTurns = 0;
        return true;
    }

    protected override void DetermineNewDestination()
    {
        base.DetermineNewDestination();
        waitAtTileTurns = Random.Range(4, 6);
    }
}
