using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMouseOver: MonoBehaviour
{
    public Entity entity;

    public void SetEntity(Entity e)
    {
        entity = e;
    }

    private void OnMouseEnter()
    {
        if (entity == null)
        {
            Debug.LogError("Null entity in EntityMouseOver component");
            return;
        }

        // If the entity is not visible, return
        if (entity.Visibility != VisibilityLevel.Visible) { return; }

        // If a dialogue is already open, don't trigger click on trader
        if (UIModality.Instance.IsDialogueOpen == true) { return; }

        ToolTipUI toolTipUI = FindObjectOfType<ToolTipUI>();
        toolTipUI.Show(entity);
    }

    private void OnMouseExit()
    {
        if (entity == null)
        {
            Debug.LogError("Null entity in EntityMouseOver component");
            return;
        }

        // If the entity is not visible, return
        if (entity.Visibility != VisibilityLevel.Visible) { return; }

        // If a dialogue is already open, don't trigger click on trader
        if (UIModality.Instance.IsDialogueOpen == true) { return; }

        ToolTipUI toolTipUI = FindObjectOfType<ToolTipUI>();
        toolTipUI.Hide(entity);
    }
}
