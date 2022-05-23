using System;
using System.Collections.Generic;

[Serializable]
public class AIEntity : Entity
{
    private Path_AStar Pathway { get; set; }
    public Tile Destination { get; private set; }
    public Tile NextTile { get; private set; }

    public AIEntity(Tile t, EntityType type) : base(t, type) { }

    // Constructor for loaded AIEntity
    public AIEntity(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, Tile t = null) : base(t, type)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Money = money;
        Visibility = visibility;
    }

    private readonly int maxTurnsNotMoved = 5;

    // TODO: Better destination determination logic
    public bool TryDetermineNewDestination()
    {
        // If pathway exists 
        if (Pathway != null)
        {
            // and not at end tile,
            // and not yet at max turns of not moving
            // return false, no new destination
            if (Pathway.Length() > 0 && TurnsNotMoved <= maxTurnsNotMoved)
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

        return true;
    }

    public void Moved()
    {
        TurnsNotMoved = 0;
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
