using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityClicker : MonoBehaviour
{
    public Entity entity;

    public void SetEntity(Entity e)
    {
        entity = e;
    }

    private void OnMouseDown()
    {
        if (entity == null)
        {
            Debug.LogError("Null entity in trader component");
            return;
        }

        // If the entity is not visible, return
        if (entity.Visibility != VisibilityLevel.Visible) { return; }

        // If a dialogue is already open, don't trigger click on trader
        if (UIModality.Instance.IsDialogueOpen == true) { return; }

        if (entity.type == EntityType.Player)
        {
            entity.PlayerClickOnPlayer();
        }
        else
        {
            // If the entity is a merchant, then click on merchant
            if (entity.GetType() == typeof(Merchant))
            {
                Merchant m = entity as Merchant;
                m.PlayerClickOnMerchant();
            }
        }
    }
}
