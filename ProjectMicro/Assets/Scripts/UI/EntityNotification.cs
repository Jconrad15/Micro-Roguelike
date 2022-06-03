using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityNotification : MonoBehaviour
{
    private GameObject canvasGO;
    private TextMeshProUGUI textObject;
    private readonly float notificationTime = 1;

    public void StartNotification(string displayText)
    {
        canvasGO = new GameObject("canvas");
        canvasGO.transform.SetParent(transform);
        canvasGO.transform.position = transform.position;

        Canvas c = canvasGO.AddComponent<Canvas>();
        c.sortingLayerName = "Text";
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        c.renderMode = RenderMode.WorldSpace;
        c.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);

        GameObject textGO = new GameObject("NotificationText");
        textObject = textGO.AddComponent<TextMeshProUGUI>();
        textGO.transform.SetParent(canvasGO.transform);
        textGO.transform.position = canvasGO.transform.position;

        textGO.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1);

        textObject.SetText(displayText);
        textObject.fontSize = 0.9f;
        textObject.color = Color.red;
        textObject.alignment = TextAlignmentOptions.Center;

        _ = StartCoroutine(FadeNotification());
    }

    private IEnumerator FadeNotification()
    {
        float timer = 0;
        while (timer <= notificationTime)
        {
            Vector2 currentPos = canvasGO.transform.position;
            currentPos.y += 0.005f;
            currentPos.x += 0.001f;
            canvasGO.transform.position = currentPos;

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

}
