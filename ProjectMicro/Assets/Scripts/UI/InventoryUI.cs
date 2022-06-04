using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private PlayerController playerController;
    private Player player;

    [SerializeField]
    private GameObject inventoryArea;
    [SerializeField]
    private GameObject itemArea;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private TextMeshProUGUI moneyText;
    [SerializeField]
    private TextMeshProUGUI tributeDueText;
    [SerializeField]
    private TextMeshProUGUI inventorySpaceText;

    private List<GameObject> inventoryItems = new List<GameObject>();

    // Start is called before the first frame update
    private void OnEnable()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.RegisterOnInventoryToggled(OnInventoryToggled);
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);

        // Start hidden
        Hide();
    }

    private void OnPlayerCreated(Player p)
    {
        p.RegisterOnPlayerClicked(OnPlayerClicked);
        p.RegisterOnLicenseChanged(OnPlayerLicenseChanged);
        p.RegisterOnMoneyChanged(OnPlayerMoneyChanged);
        p.RegisterOnItemPurchased(OnPlayerInventoryChanged);
        p.RegisterOnItemSold(OnPlayerInventoryChanged);
        player = p;
    }

    private void OnPlayerClicked(Entity obj)
    {
        OnInventoryToggled();
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
        UpdateMoneyText();
        UpdateTributeText();
        UpdateInventorySpaceText();
    }

    private void UpdateInventorySpaceText()
    {
        inventorySpaceText.SetText(
            player.InventoryItems.Count.ToString() +
            " / " + 
            player.InventorySize.ToString());
    }

    private void UpdateMoneyText()
    {
        moneyText.SetText("$" + player.Money.ToString());
    }

    private void UpdateMoneyText(int money)
    {
        moneyText.SetText("$" + money);
    }

    private void UpdateTributeText()
    {
        int tributeDue =
            TributeManager.Instance.GetTributeRatePerSeason(player.License);
        tributeDueText.SetText("$" + tributeDue.ToString());
    }

    private void CreateUIItems()
    {
        List<Item> items = player.InventoryItems;
        foreach (Item item in items)
        {
            GameObject item_GO =
                Instantiate(itemPrefab, itemArea.transform);
            
            item_GO.GetComponentInChildren<TextMeshProUGUI>()
                .SetText(item.itemName);

            // Add button function
            item_GO.GetComponent<Button>()
                .onClick.AddListener(() => OnInventoryItemClicked(item));

            inventoryItems.Add(item_GO);
        }
    }

    private void OnPlayerMoneyChanged(int money)
    {
        UpdateMoneyText(money);
    }

    private void OnPlayerLicenseChanged(Player.PlayerLicense newLicense)
    {
        UpdateTributeText();
    }

    private void OnInventoryItemClicked(Item item)
    {
        Debug.Log("Inventory item clicked");
    }

    private void OnPlayerInventoryChanged(Item item)
    {
        RefreshInventoryItems();
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

    private void RefreshInventoryItems()
    {
        foreach (GameObject item in inventoryItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        inventoryItems.Clear();

        CreateUIItems();
        UpdateInventorySpaceText();
    }
}
