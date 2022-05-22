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
        }

        // Start hidden
        Hide();
    }

    private void OnTraderClicked(Entity clickedEntity)
    {
        Show(clickedEntity);
    }

    public void Show(Entity clickedEntity)
    {
        UIModality.Instance.IsDialogueOpen = true;

        // Create player items
        List<Item> playerItems = player.InventoryItems;
        createdPlayerItems = new GameObject[playerItems.Count];
        for (int i = 0; i < playerItems.Count; i++)
        {
            GameObject newItem_GO =
                Instantiate(tradeItemPrefab, playerItemsContainer.transform);
            newItem_GO.GetComponent<TradeItemUI>()
                .Setup(playerItems[i], player, clickedEntity, true);
            createdPlayerItems[i] = newItem_GO;
        }

        // Create trader items
        List<Item> traderItems = clickedEntity.InventoryItems;
        createdTraderItems = new GameObject[traderItems.Count];
        for (int i = 0; i < traderItems.Count; i++)
        {
            GameObject newItem_GO =
                Instantiate(tradeItemPrefab, traderItemsContainer.transform);
            newItem_GO.AddComponent<TradeItemUI>()
                .Setup(traderItems[i], player, clickedEntity, false);
            createdTraderItems[i] = newItem_GO;
        }

        // Show player and trader money
        

        dialogueArea.SetActive(true);
    }

    public void Hide()
    {
        UIModality.Instance.IsDialogueOpen = false;

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

        dialogueArea.SetActive(false);
    }

}
