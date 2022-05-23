using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueArea;
    [SerializeField]
    private GameObject playerItemsContainer;
    [SerializeField]
    private GameObject traderItemsContainer;

    [SerializeField]
    private TextMeshProUGUI playerMoneyText;
    [SerializeField]
    private TextMeshProUGUI traderMoneyText;

    [SerializeField]
    private GameObject tradeItemPrefab;

    private Player player;

    private GameObject[] createdPlayerItems;
    private GameObject[] createdTraderItems;

    private void OnEnable()
    {
        WorldGenerator wg = FindObjectOfType<WorldGenerator>();
        wg.RegisterOnWorldCreated(OnWorldCreated);
        wg.RegisterOnPlayerCreated(OnPlayerCreated);
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
    }

    private void OnWorldCreated()
    {
        // Register to all traders
        List<Entity> entities = WorldData.Instance.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].RegisterOnTraderClicked(OnTraderClicked);
            player.RegisterOnPlayerClicked(OnPlayerClicked);
        }

        // Start hidden
        Hide();
    }

    private void OnTraderClicked(Entity clickedEntity)
    {
        Show(clickedEntity);
    }

    private void OnPlayerClicked(Entity playerEntity)
    {
        Debug.Log("ClickedOnPlayer");
    }

    public void Show(Entity clickedEntity)
    {
        UIModality.Instance.IsDialogueOpen = true;

        UpdateTraderPlayerItems(clickedEntity);

        // Show player and trader money
        UpdateShownMoney(clickedEntity);

        dialogueArea.SetActive(true);
    }

    private void UpdateTraderPlayerItems(Entity clickedEntity)
    {
        // Create player items
        List<Item> playerItems = player.InventoryItems;
        createdPlayerItems = new GameObject[playerItems.Count];
        for (int i = 0; i < playerItems.Count; i++)
        {
            GameObject newItem_GO =
                Instantiate(tradeItemPrefab, playerItemsContainer.transform);
            TradeItemUI tradeItemUI = newItem_GO.GetComponent<TradeItemUI>();
            tradeItemUI.Setup(playerItems[i], player, clickedEntity, true);
            tradeItemUI.RegisterOnItemTransfered(OnItemTransfered);
            createdPlayerItems[i] = newItem_GO;
        }

        // Create trader items
        List<Item> traderItems = clickedEntity.InventoryItems;
        createdTraderItems = new GameObject[traderItems.Count];
        for (int i = 0; i < traderItems.Count; i++)
        {
            GameObject newItem_GO =
                Instantiate(tradeItemPrefab, traderItemsContainer.transform);
            TradeItemUI tradeItemUI = newItem_GO.GetComponent<TradeItemUI>();
            tradeItemUI.Setup(traderItems[i], player, clickedEntity, false);
            tradeItemUI.RegisterOnItemTransfered(OnItemTransfered);
            createdTraderItems[i] = newItem_GO;
        }
    }

    /// <summary>
    /// When an item is transfered, update the money,
    /// and replace the shown items
    /// </summary>
    /// <param name="p"></param>
    /// <param name="clickedEntity"></param>
    private void OnItemTransfered(Player p, Entity clickedEntity)
    {
        UpdateShownMoney(clickedEntity);
        DestroyItems();
        UpdateTraderPlayerItems(clickedEntity);
    }

    private void UpdateShownMoney(Entity clickedEntity)
    {
        playerMoneyText.SetText("$" + player.Money.ToString());
        traderMoneyText.SetText("$" + clickedEntity.Money.ToString());
    }

    public void Hide()
    {
        UIModality.Instance.IsDialogueOpen = false;
        DestroyItems();

        dialogueArea.SetActive(false);
    }

    private void DestroyItems()
    {
        // Destroy player items
        if (createdPlayerItems != null)
        {
            for (int i = 0; i < createdPlayerItems.Length; i++)
            {
                Destroy(createdPlayerItems[i]);
            }
            createdPlayerItems = null;
        }

        // Destroy trader items
        if (createdTraderItems != null)
        {
            for (int i = 0; i < createdTraderItems.Length; i++)
            {
                Destroy(createdTraderItems[i]);
            }
            createdTraderItems = null;
        }
    }
}
