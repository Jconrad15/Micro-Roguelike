using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityNotificationGenerator : MonoBehaviour
{
    public Entity entity;

    public void SetEntity(Entity e)
    {
        entity = e;
        entity.RegisterOnMoneyDelta(OnMoneyDelta);
    }

    private void OnMoneyDelta(int delta)
    {
        CreateSmallNotification(delta);
    }

    private void CreateSmallNotification(int delta)
    {
        GameObject notification = new GameObject("MoneyDeltaNotification");
        notification.transform.SetParent(transform, false);
        notification.transform.position = transform.position;

        notification.AddComponent<EntityNotification>()
            .StartNotification(delta.ToString());
    }

    private void OnDestroy()
    {
        entity.UnregisterOnMoneyDelta(OnMoneyDelta);
    }
}
