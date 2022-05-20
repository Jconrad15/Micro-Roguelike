using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEntity : Entity
{
    private Path_AStar Pathway { get; set; }
    public Tile Destination { get; private set; }
    public Tile NextTile { get; private set; }

    public AIEntity(Tile t, EntityType type) : base(t, type) { }

    // TODO: Better destination determination logic
    public bool TryDetermineNewDestination()
    {
        // If pathway exists and
        // not at end tile, return false, no new destination
        if (Pathway != null)
        {
            if (Pathway.Length() > 0)
            {
                return false;
            }
        }

        // Determine new destination
        Destination = WorldData.Instance.GetRandomWalkableTile();

        // Create new path to destination tile
        Pathway = new Path_AStar(T, Destination);
        
        // Get rid of the first tile, this is the current tile
        _ = Pathway.Dequeue();
        
        // Set the next tile
        NextTile = Pathway.Dequeue();

        Debug.Log("New Destination is at: " + Destination.x + "," + Destination.y);
        return true;
    }

    public void Moved()
    {
        if (Pathway.Length() == 0)
        {
            NextTile = null;
        }
        else
        {
            NextTile = Pathway.Dequeue();
        }
    }
}
