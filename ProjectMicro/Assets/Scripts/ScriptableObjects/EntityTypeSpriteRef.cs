using UnityEngine;

[CreateAssetMenu(fileName = "entityTypeSpriteRef",
    menuName = "ScriptableObjects/EntityTypeSpriteRef", order = 2)]
public class EntityTypeSpriteRef : ScriptableObject
{
    public EntityType entityType;
    public Sprite[] sprites;
}
