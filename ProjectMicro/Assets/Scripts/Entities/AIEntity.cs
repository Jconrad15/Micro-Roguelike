using System;
using System.Collections.Generic;

[Serializable]
public class AIEntity : Entity
{
    protected Action<AIEntity> cbOnAIEntityRemoved;

    protected Path_AStar Pathway { get; set; }
    public Tile Destination { get; private set; }
    public Tile NextTile { get; private set; }

    public AIEntity(
        Tile t, EntityType type,
        int startingMoney, List<Attribute> attributes)
        : base(t, type, startingMoney, attributes) { }

    // Constructor for loaded AIEntity
    public AIEntity(EntityType type, List<Item> inventoryItems,
        int money, VisibilityLevel visibility, string entityName,
        string characterName, Guild guild, int favor, 
        int becomeFollowerThreshold, List<Attribute> attributes,
        Tile t = null) 
        : base(t, type, money, attributes)
    {
        base.type = type;
        InventoryItems = inventoryItems;
        Visibility = visibility;
        EntityName = entityName;
        CharacterName = characterName;
        CurrentGuild = guild;
        Favor = favor;
        T = t;
        BecomeFollowerThreshold = becomeFollowerThreshold;
    }

    protected readonly int maxTurnsNotMovedStuck = 5;

    // TODO: Better destination determination logic
    public bool TryDetermineNewDestination()
    {
        // If pathway exists 
        if (Pathway != null)
        {
            // and not at end tile,
            // and not yet at max turns of not moving
            // return false, no new destination
            if (Pathway.Length() > 0 &&
                TurnsNotMovedStuck <= maxTurnsNotMovedStuck)
            {
                return false;
            }
        }

        if (TryDetermineNewDestinationBreak() == false) { return false; }

        // Determine new destination
        DetermineNewDestination();

        // If determined destination is the same as
        // current location return false
        if (T == Destination) { return false; }
        // Create new path to destination tile
        Pathway = new Path_AStar(T, Destination);
        // Get rid of the first tile, this is the current tile
        _ = Pathway.Dequeue();
        // Set the next tile
        NextTile = Pathway.Dequeue();

        return true;
    }

    /// <summary>
    /// Determine if the entity should not move. 
    /// Returns false to not move.
    /// </summary>
    /// <returns></returns>
    protected virtual bool TryDetermineNewDestinationBreak()
    {
        // Any early breaks for the AIEntity can be placed in here.

        return true;
    }

    protected virtual void DetermineNewDestination()
    {
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        Destination = areaData.GetRandomWalkableTile();
    }

    public void Moved()
    {
        TurnsNotMovedStuck = 0;
        if (Pathway.Length() == 0)
        {
            NextTile = null;
        }
        else
        {
            NextTile = Pathway.Dequeue();
        }
    }

    public void RemoveFromAreaToBeFollower()
    {
        AreaDataManager.Instance.RemoveEntityFromCurrentAreaData(this);
        cbOnAIEntityRemoved?.Invoke(this);
        ClearData();
        NullPathfinding();
    }

    public void NullPathfinding()
    {
        Destination = null;
        NextTile = null;
        Pathway = null;
    }

    public override void ClearData()
    {
        base.ClearData();
        Pathway = null;
        Destination = null;
        NextTile = null;
    }

    public void RegisterOnAIEntityRemoved(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityRemoved += callbackfunc;
    }

    public void UnregisterOnAIEntityRemoved(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityRemoved -= callbackfunc;
    }
}
