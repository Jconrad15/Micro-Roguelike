using UnityEngine;

[CreateAssetMenu(fileName = "traitSpriteRef",
    menuName = "ScriptableObjects/traitSpriteRef", order = 8)]
public class TraitSpriteRef : ScriptableObject
{
    public string traitName;
    public Sprite sprite;
}
