using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public Dictionary<string, Item> ItemRefDatabase { get; protected set; }

    public void CreateDatabase()
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
            ItemRefDatabase.Add(itemRefs[i].itemName, new Item(
                itemRefs[i].itemName, itemRefs[i].baseCost,itemRefs[i].category));
        }
    }
}