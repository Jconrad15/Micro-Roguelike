using UnityEngine;

[CreateAssetMenu(fileName = "merchantTypeRef",
    menuName = "ScriptableObjects/MerchantTypeRef", order = 5)]
public class MerchantTypeRef : ScriptableObject
{
    public MerchantType merchantType;
    public ItemRef[] preferredBuy;
    public ItemRef[] preferredSell;

    public Sprite overrideSprite;
}
