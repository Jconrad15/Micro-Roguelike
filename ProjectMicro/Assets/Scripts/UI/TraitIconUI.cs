using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class TraitIconUI : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image image;

    private Trait trait;

    private GameObject traitToolTip;

    public void Setup(Trait trait)
    {
        this.trait = trait;

        SpriteDatabase spriteDatabase =
            FindObjectOfType<SpriteDatabase>();

        if (spriteDatabase.TraitDatabase.
            TryGetValue(trait.TraitName, out Sprite s))
        {
            image.sprite = s;
        }
        else
        {
            Debug.LogError("No sprite for this trait");
        }

    }

    private void ShowTraitTooltip()
    {
        traitToolTip = new GameObject("traitToolTip");
        traitToolTip.transform.SetParent(transform);

        RectTransform rt = traitToolTip.AddComponent<RectTransform>();
        rt.anchoredPosition = Vector3.zero;
        rt.sizeDelta = new Vector2(200f, 100f);

        Image tttImage = traitToolTip.AddComponent<Image>();
        tttImage.color = new Color32(0, 0, 0, 150);

        GameObject textObject = new GameObject("text");
        textObject.transform.SetParent(traitToolTip.transform);

        textObject.AddComponent<RectTransform>()
            .anchoredPosition = Vector3.zero;

        TextMeshProUGUI textMesh =
            textObject.AddComponent<TextMeshProUGUI>();

        textMesh.SetText(trait.TraitName + "\n" + trait.Description);
        textMesh.fontSize = 18;
    }

    private void HideTraitTooltip()
    {
        Destroy(traitToolTip);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTraitTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTraitTooltip();
    }
}
