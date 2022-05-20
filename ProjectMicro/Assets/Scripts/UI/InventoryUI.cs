using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UI
{
    private PlayerController playerController;
    [SerializeField]
    private GameObject inventoryArea;

    // Start is called before the first frame update
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.RegisterOnInventoryToggled(OnInventoryToggled);

        // Start hidden
        Hide(inventoryArea);
    }

    private void OnInventoryToggled()
    {
        if (UIModality.Instance.IsInventoryOpen)
        {
            Hide(inventoryArea);
        }
        else
        {
            Show(inventoryArea);
        }
    }

}
