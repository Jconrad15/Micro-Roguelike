using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textObject;

    private CanvasGroup canvasGroup;
    private float fadeSpeed = 0.5f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();

        SaveSerial.Instance.RegisterOnDataLoaded(OnDataLoaded);
        SaveSerial.Instance.RegisterOnDataSaved(OnDataSaved);
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
    }

    private void OnDataLoaded(LoadedWorldData obj)
    {
        SetText("Data Loaded Successfully.");
    }

    private void OnDataSaved()
    {
        SetText("Data Saved Successfully.");
    }

    private void SetText(string text)
    {
        textObject.SetText(text);
        Show();
        StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        Hide();
    }

}
