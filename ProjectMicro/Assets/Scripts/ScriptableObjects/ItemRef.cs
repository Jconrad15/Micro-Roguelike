using UnityEngine;

[CreateAssetMenu(fileName = "itemRef",
    menuName = "ScriptableObjects/ItemRef", order = 4)]
public class ItemRef : ScriptableObject
{
    public string itemName;
    public int baseCost;
    public ItemCategory category;
}
