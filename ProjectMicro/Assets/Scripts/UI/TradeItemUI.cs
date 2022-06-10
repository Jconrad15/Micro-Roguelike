using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradeItemUI : MonoBehaviour
{
    [SerializeField]
    private Color goodDealColor;
    [SerializeField]
    private Color badDealColor;
    [SerializeField]
    private Color baseColor;

    [SerializeField]
    private TextMeshProUGUI itemNameText;

    [SerializeField]
    private TextMeshProUGUI buttonText;
    
    [SerializeField]
    private Button button;

    private Entity clickedEntity;
    private Player player;
    private Item item;
    private bool isPlayerItem;

    private Action<Player, Entity> onItemTransfered;

    public void Setup(Item item, Player player,
        Entity clickedEntity, bool isPlayerItem)
    {
        this.item = item;
        this.clickedEntity = clickedEntity;
        this.player = player;
        this.isPlayerItem = isPlayerItem;

        itemNameText.SetText(item.itemName);

        Merchant m = (Merchant)clickedEntity;
        int adjustedCost = m.GetAdjustedCost(item, player, isPlayerItem);

        if (isPlayerItem)
        {
            if (adjustedCost > item.baseCost)
            {
                buttonText.color = goodDealColor;
            }
            else if (adjustedCost < item.baseCost)
            {
                buttonText.color = badDealColor;
            }
            else
            {
                buttonText.color = baseColor;
            }
        }
        else
        {
            if (adjustedCost > item.baseCost)
            {
                buttonText.color = badDealColor;
            }
            else if (adjustedCost < item.baseCost)
            {
                buttonText.color = goodDealColor;
            }
            else
            {
                buttonText.color = baseColor;
            }
        }

        buttonText.SetText("$" + adjustedCost.ToString());

        // Setup the button
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // Check if the clicked entity object can transfer the item
        bool itemTransfered = clickedEntity.TryTransferItem(
            item, player, clickedEntity, isPlayerItem);
        
        if (itemTransfered)
        {
            // Do something if the item is transferred
            // Need to update the UI list
            onItemTransfered?.Invoke(player, clickedEntity);
        }
    }

    public void RegisterOnItemTransfered(
        Action<Player, Entity> callbackfunc)
    {
        onItemTransfered += callbackfunc;
    }

    public void UnregisterOnItemTransfered(
        Action<Player, Entity> callbackfunc)
    {
        onItemTransfered -= callbackfunc;
    }
}
