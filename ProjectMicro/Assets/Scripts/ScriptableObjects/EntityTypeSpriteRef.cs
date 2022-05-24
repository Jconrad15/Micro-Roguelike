using UnityEngine;

[CreateAssetMenu(fileName = "entityTypeSpriteRef",
    menuName = "ScriptableObjects/EntityTypeSpriteRef", order = 2)]
public class EntityTypeSpriteRef : ScriptableObject
{
    public string entityName;
    public Sprite[] sprites;
}
