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

        WorldGenerator.Instance.RegisterOnWorldCreated(OnWorldCreated);
    }

    private void OnWorldCreated()
    {
        AreaDataManager.Instance
            .RegisterOnCurrentMapTypeChange(OnCurrentMapTypeChange);
        OnCurrentMapTypeChange(MapType.World);
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
    }

    private void OnDataLoaded(LoadedAreaData obj)
    {
        SetText("Data Loaded Successfully.");
    }

    private void OnDataSaved()
    {
        SetText("Data Saved Successfully.");
    }

    private void OnCurrentMapTypeChange(MapType mapType)
    {
        List<Entity> entities = AreaData.GetEntitiesForCurrentType();

        foreach (Entity entity in entities)
        {
            if (entity == null) { continue; }

            entity.RegisterOnPurchaseFailedInventory(OnPurchaseFailedInventory);
            entity.RegisterOnPurchaseFailedMoney(OnPurchaseFailedMoney);
        }
    }

    private void OnPurchaseFailedInventory()
    {
        SetText("Not enough inventory space.");
    }

    private void OnPurchaseFailedMoney()
    {
        SetText("Not enough money.");
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
            yield return new WaitForEndOfFrame();
        }

        Hide();
    }

}
