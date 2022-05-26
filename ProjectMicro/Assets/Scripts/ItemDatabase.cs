using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDatabase
{
    public static Dictionary<string, Item> ItemRefDatabase
    { get; private set; }

    public static void CreateDatabase()
    {
        // Load scriptable objects from file

        // Create item ref database
        ItemRef[] itemRefs =
            Resources.LoadAll("ScriptableObjects/Items",
            typeof(ItemRef))
            .Cast<ItemRef>().ToArray();

        ItemRefDatabase = new Dictionary<string, Item>();
        // Create the database
        for (int i = 0; i < itemRefs.Length; i++)
        {
            ItemRefDatabase.Add(
                itemRefs[i].itemName,
                new Item(
                    itemRefs[i].itemName, itemRefs[i].baseCost, 
                    itemRefs[i].category));
        }
    }

    public static Item GetRandomItem()
    {
        // TODO: store this items list? so that it isn't created each time
        List<Item> items = new List<Item>(ItemRefDatabase.Values);
        return items[Random.Range(0, items.Count)];
    }

    public static Item GetItemByName(string name)
    {
        if (ItemRefDatabase.TryGetValue(name, out Item item))
        {
            return item;
        }

        return null;
    }
}