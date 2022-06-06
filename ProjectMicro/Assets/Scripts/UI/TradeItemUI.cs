using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradeItemUI : MonoBehaviour
{
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
        int adjustedCost = m.GetAdjustedCost(item);

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

    public void RegisterOnItemTransfered(Action<Player, Entity> callbackfunc)
    {
        onItemTransfered += callbackfunc;
    }

    public void UnregisterOnItemTransfered(Action<Player, Entity> callbackfunc)
    {
        onItemTransfered -= callbackfunc;
    }
}
