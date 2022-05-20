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

    private List<GameObject> inventoryitems = new List<GameObject>();

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


    public void Show(GameObject go)
    {
        go.SetActive(true);
        CreateUIItems();
    }

    private void CreateUIItems()
    {
        List<Item> items = playerController.GetPlayer().InventoryItems;
        foreach (Item item in items)
        {
            GameObject item_GO = Instantiate(itemPrefab, inventoryArea.transform);
            item_GO.GetComponent<TextMeshProUGUI>().SetText(item.name);
            inventoryitems.Add(item_GO);
        }
    }

    public void Hide(GameObject go)
    {
        go.SetActive(false);
        foreach (GameObject item in inventoryitems)
        {
            Destroy(item);
        }
        inventoryitems.Clear();
    }
}
