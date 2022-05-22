using UnityEngine;

[CreateAssetMenu(fileName = "featureTypeSpriteRef",
    menuName = "ScriptableObjects/FeatureTypeSpriteRef", order = 3)]
public class FeatureTypeSpriteRef : ScriptableObject
{
    public FeatureType featureType;
    public Sprite[] sprites;
}
