using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    [SerializeField]
    private GameObject toolTipArea;
    private RectTransform rect;
    private Vector3 min, max;
    private readonly float offset = 10f;

    void Start()
    {
        rect = toolTipArea.GetComponent<RectTransform>();
        min = new Vector3(0, 0, 0);
        max = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 0);

        Hide();
    }

    public void Show(Entity e)
    {

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
        Vector3 position = new Vector3(
            Input.mousePosition.x + rect.rect.width,
            Input.mousePosition.y - (rect.rect.height / 2 + offset),
            0f);

        // Clamp it to the screen size so it doesn't go outside
        transform.position = new Vector3(
            Mathf.Clamp(position.x, min.x + rect.rect.width / 2,
                        max.x - rect.rect.width / 2), 
            Mathf.Clamp(position.y, min.y + rect.rect.height / 2,
                        max.y - rect.rect.height / 2),
            transform.position.z);

        toolTipArea.SetActive(true);
    }

    private void Hide()
    {
        toolTipArea.SetActive(false);
    }

}
