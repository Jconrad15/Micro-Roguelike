using System;

public static class PlayerInstantiation
{
    private static Action<Player> cbOnPlayerCreated;

    public static void CreatePlayer(int playerStartX, int playerStartY)
    {
        // Get the tile at the location
        Tile playerTile;
        if (CurrentMapType.Type == MapType.Location) 
        {
            playerTile = LocationData.Instance.GetTile(playerStartX, playerStartY);
        }
        else
        {
            playerTile = WorldData.Instance.GetTile(playerStartX, playerStartY);
        }

        // Place the player at the tile, and the tile to the player
        Player player = new Player(playerTile, EntityType.Player, 10);
        playerTile.entity = player;

        // TODO: Starting player items
        player.InventoryItems.Add(ItemDatabase.GetRandomItem());

        cbOnPlayerCreated?.Invoke(player);
    }

    public static void RegisterOnPlayerCreated(Action<Player> callbackfunc)
    {
        cbOnPlayerCreated += callbackfunc;
    }

    public static void UnregisterOnPlayerCreated(Action<Player> callbackfunc)
    {
        cbOnPlayerCreated -= callbackfunc;
    }
}
