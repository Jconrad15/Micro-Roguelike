using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    private TextMeshProUGUI merchantMoneyText;
    [SerializeField]
    private TextMeshProUGUI entityTitleText;

    [SerializeField]
    private GameObject tradeItemPrefab;

    [SerializeField]
    private TextMeshProUGUI playerGuildText;
    [SerializeField]
    private TextMeshProUGUI aiGuildText;

    [SerializeField]
    private TextMeshProUGUI favorText;

    [SerializeField]
    private GameObject recruitContainer;

    private Player player;

    private GameObject[] createdPlayerItems;
    private GameObject[] createdTraderItems;

    private void OnEnable()
    {
        FindObjectOfType<LocationGenerator>()
            .RegisterOnLocationCreated(OnLocationCreated);
        FindObjectOfType<WorldGenerator>()
            .RegisterOnWorldCreated(OnWorldCreated);
        AreaDataManager.Instance
            .RegisterOnCurrentMapTypeChange(OnCurrentMapTypeChange);

        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);

        // Start hidden
        Hide();
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
        player.RegisterOnFollowerAdded(OnFollowerAdded);
    }

    private void OnWorldCreated()
    {
        RegisterToClicksOnEntities(MapType.World);
    }

    private void OnLocationCreated()
    {
        RegisterToClicksOnEntities(MapType.Location);
    }

    private void RegisterToClicksOnEntities(MapType mapType)
    {
        // Determine entities using maptype
        List<Entity> entities;
        if (mapType == MapType.World)
        {
            entities = AreaDataManager.Instance.GetWorldData().Entities;
        }
        else
        {
            entities = AreaDataManager.Instance.CurrentLocationData.Entities;
        }

        // Register to all merchants
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null) { continue; }

            if (entities[i].GetType() == typeof(Merchant))
            {
                entities[i].RegisterOnMerchantClicked(OnMerchantClicked);
            }
        }
    }

    public void UnregisterToClicksOnEntities()
    {
        MapType mapType = AreaDataManager.Instance.CurrentMapType;

        List<Entity> entities;
        if (mapType == MapType.World)
        {
            entities = AreaDataManager.Instance.GetWorldData().Entities;
        }
        else
        {
            entities = AreaDataManager.Instance.CurrentLocationData.Entities;
        }

        // Unregister to all merchants
        for (int i = 0; i < entities.Count; i++)
        {
            if (entities[i] == null) { continue; }

            if (entities[i].GetType() == typeof(Merchant))
            {
                entities[i].UnregisterOnMerchantClicked(OnMerchantClicked);
            }
        }
    }

    private void OnMerchantClicked(Entity clickedEntity)
    {
        Show(clickedEntity);
    }

    public void Show(Entity clickedEntity)
    {
        UIModality.Instance.IsDialogueOpen = true;

        UpdateTraderPlayerItems(clickedEntity);

        // Show player and trader money
        UpdateShownMoney(clickedEntity);
        UpdateTitle(clickedEntity);
        UpdateShownGuilds(clickedEntity);
        UpdateShownFavor(clickedEntity);
        dialogueArea.SetActive(true);
    }

    private void UpdateShownFavor(Entity clickedEntity)
    {
        int favor = clickedEntity.Favor;
        string text = "Favor: ";
        if (favor > 0) { text += "+"; }
        text += favor.ToString();

        favorText.SetText(text);

        // Check clickedEntity can become a follower
        if (favor >= clickedEntity.BecomeFollowerThreshold)
        {
            ShowBecomeFollower(clickedEntity);
        }
    }

    private void ShowBecomeFollower(Entity clickedEntity)
    {
        Button recruitButton =
            recruitContainer.GetComponentInChildren<Button>();

        recruitButton.onClick.AddListener(
            () => player.TryAddFollower(clickedEntity));

        recruitContainer.SetActive(true);
    }

    private void HideBecomeFollower()
    {
        Button recruitButton =
            recruitContainer.GetComponentInChildren<Button>();

        recruitButton.onClick.RemoveAllListeners();

        recruitContainer.SetActive(false);
    }

    private void OnFollowerAdded(Follower f)
    {
        Hide();
    }

    private void UpdateTitle(Entity clickedEntity)
    {
        Merchant merchant = (Merchant)clickedEntity;

        string text = merchant.CharacterName + " the " +
                      merchant.MType.ToString();

        entityTitleText.SetText(text);
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
        UpdateShownFavor(clickedEntity);
    }

    private void UpdateShownMoney(Entity clickedEntity)
    {
        playerMoneyText.SetText("$" + player.Money.ToString());
        merchantMoneyText.SetText("$" + clickedEntity.Money.ToString());
    }

    private void UpdateShownGuilds(Entity clickedEntity)
    {
        if (player.CurrentGuild == null)
        {
            Debug.LogWarning("no player guild");
            return;
        }

        if (clickedEntity.CurrentGuild == null)
        {
            Debug.LogWarning("no ai guild");
            return;
        }

        playerGuildText.SetText(player.CurrentGuild.GuildName);
        aiGuildText.SetText(clickedEntity.CurrentGuild.GuildName);
    }

    public void Hide()
    {
        UIModality.Instance.IsDialogueOpen = false;
        DestroyItems();
        HideBecomeFollower();
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

    private void OnCurrentMapTypeChange(MapType mapType)
    {
        UnregisterToClicksOnEntities();
    }
}
