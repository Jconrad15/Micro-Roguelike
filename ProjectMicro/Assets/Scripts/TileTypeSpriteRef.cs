using UnityEngine;

[CreateAssetMenu(fileName = "tileTypeSpriteRef",
    menuName = "ScriptableObjects/TileTypeSpriteRef", order = 1)]
public class TileTypeSpriteRef : ScriptableObject
{
    public TileType tileType;
    public Sprite sprite;
}
