using System;

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
        Merchant merchant = new Merchant(merchantTile, EntityType.AI, 10);
        merchantTile.entity = merchant;
        cbOnAIEntityCreated?.Invoke(merchant);
    }

    public static void LoadEntity(AIEntity loadedEntity)
    {
        cbOnAIEntityCreated?.Invoke(loadedEntity);
    }

    public static void RegisterOnAIEntityCreated(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated += callbackfunc;
    }

    public static void UnregisterOnAIEntityCreated(Action<AIEntity> callbackfunc)
    {
        cbOnAIEntityCreated -= callbackfunc;
    }
}
