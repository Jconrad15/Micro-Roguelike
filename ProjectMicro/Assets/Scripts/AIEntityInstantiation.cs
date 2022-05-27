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

        // Create travelling merchants
        int width = WorldData.Instance.Width;
        int height = WorldData.Instance.Height;
        int merchantCount = 10;

        for (int i = 0; i < merchantCount; i++)
        {
            int x = 0;
            int y = 0;
            bool isPositionDetermined = false;
            int breakCounter = 0;
            while (isPositionDetermined == false)
            {
                x = Random.Range(0, width);
                y = Random.Range(0, height);

                Tile testTile = WorldData.Instance.GetTile(x, y);
                if (testTile.entity == null)
                {
                    isPositionDetermined = true;
                }
                else
                {
                    breakCounter++;
                    if (breakCounter == width * height)
                    {
                        goto NoMoreOpenPositions;
                    }
                }
            }

            Tile merchantTile = WorldData.Instance.GetTile(x, y);
            Merchant merchant =
                new Merchant(merchantTile, EntityType.AI, 10);

            merchantTile.entity = merchant;
            cbOnAIEntityCreated?.Invoke(merchant);
        }

        NoMoreOpenPositions:
        // Also create a dog to wander around
        Tile dogTile = WorldData.Instance.GetTile(1, 1);
        Dog dog = new Dog(dogTile, EntityType.AI, 0);
        dogTile.entity = dog;
        cbOnAIEntityCreated?.Invoke(dog);

        Random.state = oldState;
    }

    public static void CreateAIEntities(int width, int height)
    {
        // Get the area data
        AreaData areaData = AreaData.GetAreaDataForCurrentType();

        // For now generate a dog
        Tile dogTile = areaData.GetTile(1, 1);
        Dog dog = new Dog(dogTile, EntityType.AI, 0);
        dogTile.entity = dog;
        cbOnAIEntityCreated?.Invoke(dog);

        // Also create a Merchant
        Tile merchantTile = areaData.GetTile(2, 1);
        Merchant merchant =
            new Merchant(merchantTile, EntityType.AI, 10);

        merchantTile.entity = merchant;
        cbOnAIEntityCreated?.Invoke(merchant);
    }

    public static void GetPreviousWorldEntities()
    {
        List<Entity> prevEntities =
            AreaData.GetEntitiesForCurrentType();

        foreach (Entity entity in prevEntities)
        {
            if (Utility.IsSameOrSubclass(typeof(AIEntity), entity.GetType()))
            {
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
                loadedEntity.EntityName, loadedEntity.CharacterName);
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
