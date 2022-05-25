using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField]
    private GameObject toolTipArea;
    [SerializeField]
    private TextMeshProUGUI entityText;
    [SerializeField]
    private TextMeshProUGUI nameText;

    private RectTransform rt;
    private Vector3 minScreenSize, maxScreenSize;
    private readonly float yOffset = 20f;
    private readonly float xOffset = 20f;

    void Start()
    {
        rt = toolTipArea.GetComponent<RectTransform>();
        minScreenSize = new Vector2(0, 0);
        maxScreenSize = new Vector2(Screen.width, Screen.height);

        Hide();
    }

    public void Show(Entity e)
    {
        if (e == null) return;

        entityText.SetText(e.GetType().ToString());
        nameText.SetText(e.CharacterName);

        Show();
    }

    public void Hide(Entity e)
    {

        Hide();
    }

    private void Show()
    {
        // Position the tooltip
        // Get the tooltip position with offset
        Vector2 position = new Vector2(
            Input.mousePosition.x + xOffset,
            Input.mousePosition.y - rt.rect.height - yOffset);

        Debug.Log("Position" + position);
        
        // Clamp it to the screen size so it doesn't go outside
        rt.anchoredPosition = new Vector2(
            Mathf.Clamp(position.x, minScreenSize.x + (rt.rect.width / 2),
                        maxScreenSize.x - rt.rect.width / 2), 
            Mathf.Clamp(position.y, minScreenSize.y + (rt.rect.height / 2),
                        maxScreenSize.y - rt.rect.height / 2));
        Debug.Log("AnchoredPosition" + rt.anchoredPosition);

        toolTipArea.SetActive(true);
    }

    private void Hide()
    {
        toolTipArea.SetActive(false);
    }

}
