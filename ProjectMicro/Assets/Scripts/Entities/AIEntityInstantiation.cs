using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class AIEntityInstantiation
{
    private static Action<AIEntity> cbOnAIEntityCreated;

    public static void CreateInitialWorldEntities(int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        AreaData worldData = AreaDataManager.Instance.GetWorldData();

        // Create travelling merchants
        int width = worldData.Width;
        int height = worldData.Height;
        int merchantCount = 10;

        for (int i = 0; i < merchantCount; i++)
        {
            DetermineOpenLocation(worldData, out int x, out int y);
            if (x == int.MinValue || y == int.MinValue) { break; }

            int merchantStartMoney = Random.value < 0.9 ?
                Random.Range(5, 12) :
                Random.Range(20, 30);

            Tile merchantTile = worldData.GetTile(x, y);
            Merchant merchant = new Merchant(
                merchantTile,
                EntityType.AI,
                MerchantType.Traveller,
                merchantStartMoney);

            merchantTile.entity = merchant;
            cbOnAIEntityCreated?.Invoke(merchant);
        }

        // Also create a dog to wander around
        Tile dogTile = worldData.GetTile(1, 1);
        Dog dog = new Dog(dogTile, EntityType.AI, 0);
        dogTile.entity = dog;
        cbOnAIEntityCreated?.Invoke(dog);

        Random.state = oldState;
    }

    public static void LoadLocationAIEntities(AreaData locationData)
    {
        List<Entity> entities = locationData.Entities;

        for (int i = 0; i < entities.Count; i++)
        {
            Entity e = entities[i];

            if (Utility.IsSameOrSubclass(typeof(AIEntity), e.GetType()))
            {
                // Set new tile so that the old tiles reference is lost
                e.SetTile(locationData.GetTile(e.X, e.Y));
                // Convert AIEntity to subclass
                dynamic obj = Convert.ChangeType(e, e.GetType());
                cbOnAIEntityCreated?.Invoke(obj);
                continue;
            }

            // Delete the player entity 
            if (Utility.IsSameOrSubclass(typeof(Player), e.GetType()))
            {
                e.ClearData();
                locationData.Entities.Remove(e);
                continue;
            }
        }
    }

    public static void CreateLocationAIEntities(int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        // Get the area data
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        int aiEntityCount = Random.Range(2, 6);
        for (int i = 0; i < aiEntityCount; i++)
        {
            DetermineOpenLocation(areaData, out int x, out int y);
            if (x == int.MinValue || y == int.MinValue) { break; }

            int merchantStartMoney = Random.value < 0.9 ?
                Random.Range(5, 12) :
                Random.Range(20, 30);

            Tile merchantTile = areaData.GetTile(x, y);
            Merchant merchant = new Merchant(
                merchantTile,
                EntityType.AI,
                Utility.GetRandomEnum<MerchantType>(),
                merchantStartMoney);

            merchantTile.entity = merchant;
            cbOnAIEntityCreated?.Invoke(merchant);
        }

        // Also generate a dog
        Tile dogTile = areaData.GetTile(1, 1);
        Dog dog = new Dog(dogTile, EntityType.AI, 0);
        dogTile.entity = dog;
        cbOnAIEntityCreated?.Invoke(dog);

        Random.state = oldState;
    }

    private static void DetermineOpenLocation(
        AreaData ad, out int x, out int y)
    {
        x = 0;
        y = 0;
        bool isPositionDetermined = false;
        int breakCounter = 0;
        while (isPositionDetermined == false)
        {
            x = Random.Range(0, ad.Width);
            y = Random.Range(0, ad.Height);

            Tile testTile = ad.GetTile(x, y);
            if (testTile.entity == null)
            {
                isPositionDetermined = true;
            }
            else
            {
                breakCounter++;
                if (breakCounter == ad.Width * ad.Height)
                {
                    x = int.MinValue;
                    y = int.MinValue;
                    break;
                }
            }
        }
    }

    public static void LoadPreviousWorldEntities()
    {
        List<Entity> prevEntities =
            AreaData.GetEntitiesForCurrentType();

        AreaData worldData = AreaDataManager.Instance.GetWorldData();

        foreach (Entity entity in prevEntities)
        {
            if (Utility.IsSameOrSubclass(typeof(AIEntity), entity.GetType()))
            {
                // Set new tile so that the old tiles reference is lost
                entity.SetTile(worldData.GetTile(entity.X, entity.Y));
                // Convert AIEntity to subclass
                dynamic obj = Convert.ChangeType(entity, entity.GetType());
                cbOnAIEntityCreated?.Invoke(obj);
            }
        }
    }

    public static void LoadEntity(Entity loadedEntity)
    {
        // TODO: Need a better way to convert loaded entity
        // to specific type of ai entity
        if (loadedEntity.EntityName == "dog")
        {
            Dog dog = new Dog(
                loadedEntity.type, loadedEntity.InventoryItems,
                loadedEntity.Money, loadedEntity.Visibility,
                loadedEntity.EntityName, loadedEntity.CharacterName);
            dog.SetTile(loadedEntity.T);
            loadedEntity.T.entity = dog;

            AreaData.GetAreaDataForCurrentType().AddEntity(dog);
            cbOnAIEntityCreated?.Invoke(dog);
        }
        else if (loadedEntity.EntityName == "merchant")
        {
            Merchant merchant = new Merchant(
                loadedEntity.type, loadedEntity.InventoryItems,
                loadedEntity.Money, loadedEntity.Visibility,
                loadedEntity.EntityName, loadedEntity.CharacterName,
                loadedEntity.CurrentGuild, loadedEntity.BecomeFollowerThreshold);
            merchant.SetTile(loadedEntity.T);
            loadedEntity.T.entity = merchant;

            AreaData.GetAreaDataForCurrentType().AddEntity(merchant);
            cbOnAIEntityCreated?.Invoke(merchant);
        }
        else
        {
            Debug.Log("Missing entity name of: " +
                loadedEntity.EntityName);
        }
    }

    public static void RegisterOnAIEntityCreated(
        Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated += callbackfunc;
    }

    public static void UnregisterOnAIEntityCreated(
        Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated -= callbackfunc;
    }
}
