using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private PlayerController playerController;
    
    [SerializeField]
    private GameObject inventoryArea;
    [SerializeField]
    private GameObject itemPrefab;

    private List<GameObject> inventoryItems = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.RegisterOnInventoryToggled(OnInventoryToggled);

        // Start hidden
        Hide();
    }

    private void OnInventoryToggled()
    {
        UIModality.Instance.ToggleInventoryOpen();
        if (UIModality.Instance.IsInventoryOpen)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        inventoryArea.SetActive(true);
        CreateUIItems();
    }

    private void CreateUIItems()
    {
        List<Item> items = playerController.GetPlayer().InventoryItems;
        foreach (Item item in items)
        {
            GameObject item_GO = Instantiate(itemPrefab, inventoryArea.transform);
            item_GO.GetComponentInChildren<TextMeshProUGUI>().SetText(item.name);
            inventoryItems.Add(item_GO);
        }
    }

    public void Hide()
    {
        inventoryArea.SetActive(false);
        foreach (GameObject item in inventoryItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        inventoryItems.Clear();
    }
}
