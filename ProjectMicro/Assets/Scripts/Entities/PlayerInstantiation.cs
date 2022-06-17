using System;

public static class PlayerInstantiation
{
    private static Action<Player> cbOnPlayerCreated;

    private static int playerStartingMoney = 100;

    public static void TransitionPlayerToMap(
        Player player, int playerStartX, int playerStartY)
    {
        // Get the tile at the new player location
        Tile playerTile;
        if (AreaDataManager.Instance.CurrentMapType == MapType.Location)
        {
            playerTile = AreaDataManager.Instance.CurrentLocationData
                .GetTile(playerStartX, playerStartY);
        }
        else
        {
            playerTile = AreaDataManager.Instance.GetWorldData()
                .GetTile(playerStartX, playerStartY);
        }

        // Place the player at the tile, and the tile to the player
        playerTile.entity = player;
        player.SetTile(playerTile);

        // Add player to entities list
        //AreaData.GetAreaDataForCurrentType().AddEntity(player);

        cbOnPlayerCreated?.Invoke(player);
    }

    public static void CreatePlayer(
        Player createdPlayer, int playerStartX, int playerStartY)
    {

        // Get the tile at the location
        Tile playerTile;
        if (AreaDataManager.Instance.CurrentMapType == MapType.Location) 
        {
            playerTile = AreaDataManager.Instance.CurrentLocationData
                .GetTile(playerStartX, playerStartY);
        }
        else
        {
            playerTile = AreaDataManager.Instance.GetWorldData()
                .GetTile(playerStartX, playerStartY);
        }

        // Place the player at the tile, and the tile to the player
        Player player = new Player(
            playerTile, EntityType.Player,
            playerStartingMoney, createdPlayer,
            createdPlayer.Traits);

        playerTile.entity = player;

        // Add player to entities list
        AreaData.GetAreaDataForCurrentType().AddEntity(player);

        // TODO: Starting player items
        player.InventoryItems.Add(ItemDatabase.GetRandomItem());

        cbOnPlayerCreated?.Invoke(player);
    }

    public static void LoadPlayer(Entity entityToLoad)
    {
        // Get the tile at the location
        Tile playerTile;
        if (AreaDataManager.Instance.CurrentMapType == MapType.Location)
        {
            playerTile = AreaDataManager.Instance.CurrentLocationData
                .GetTile(entityToLoad.X, entityToLoad.Y);
        }
        else
        {
            playerTile = AreaDataManager.Instance.GetWorldData()
                .GetTile(entityToLoad.X, entityToLoad.Y);
        }

        // Place the player at the tile, and the tile to the player
        Player player = new Player(
            EntityType.Player, entityToLoad.InventoryItems,
            entityToLoad.Money, entityToLoad.Visibility,
            entityToLoad.EntityName, entityToLoad.CharacterName,
            entityToLoad.CurrentGuild, entityToLoad.Traits,
            playerTile);

        playerTile.entity = player;
                AreaData.GetAreaDataForCurrentType().AddEntity(player);

        // Add player to entities list
        AreaData.GetAreaDataForCurrentType().AddEntity(player);

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
