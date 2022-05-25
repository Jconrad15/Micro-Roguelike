using System;
using UnityEngine;

public static class AIEntityInstantiation
{
    private static Action<AIEntity> cbOnAIEntityCreated;

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
