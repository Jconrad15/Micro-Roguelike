using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityAlphaChanger: MonoBehaviour
{
    private static Dictionary<SpriteRenderer, Coroutine> currentlyLerpingSRs =
        new Dictionary<SpriteRenderer, Coroutine>();

    private readonly float alphaChangeSpeed = 0.8f;

    private void OnEnable()
    {
        AreaDataManager.Instance.RegisterOnCurrentMapTypeChange(OnCurrentMapTypeChange);
    }

    public void ChangeVisibilityAlpha(Feature feature, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)feature.Visibility / 2f);
    }

    public void ChangeVisibilityAlpha(Tile tile, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)tile.Visibility / 2f);
    }

    public void ChangeVisibilityAlpha(Entity entity, SpriteRenderer sr)
    {
        ChangeVisibilityAlpha(sr, (int)entity.Visibility / 2f);
    }

    public void ChangeVisibilityAlpha(SpriteRenderer sr, float targetAlpha)
    {
        if (sr == null)
        {
            Debug.Log("Null sprite renderer");
            return;
        }

        if (currentlyLerpingSRs.ContainsKey(sr))
        {
            currentlyLerpingSRs.TryGetValue(sr, out Coroutine runningCR);
            if (runningCR != null)
            {
                StopCoroutine(runningCR);
            }
            currentlyLerpingSRs.Remove(sr);
        }

        Coroutine cr = StartCoroutine(LerpVisibility(sr, targetAlpha));
        currentlyLerpingSRs.Add(sr, cr);
    }

    private IEnumerator LerpVisibility(SpriteRenderer sr, float targetAlpha)
    {
        Color currentColor = sr.color;
        Color targetColor = currentColor;
        targetColor.a = targetAlpha;
        float t = 0;
        while (Mathf.Abs(currentColor.a - targetColor.a) > 0.01)
        {
            currentColor.a = Mathf.Lerp(currentColor.a, targetColor.a, t);
            sr.color = currentColor;
            t += alphaChangeSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        currentlyLerpingSRs.Remove(sr);
        sr.color = targetColor;
    }

    private void OnCurrentMapTypeChange(MapType obj)
    {
        StopAllCoroutines();
        currentlyLerpingSRs.Clear();
    }
}
