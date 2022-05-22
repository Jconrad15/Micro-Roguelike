using System.Collections;
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

    public void Setup(Item item, Player player,
        Entity clickedEntity, bool isPlayerItem)
    {
        this.item = item;
        this.clickedEntity = clickedEntity;
        this.player = player;
        this.isPlayerItem = isPlayerItem;

        itemNameText.SetText(item.name);
        buttonText.SetText("$" + item.baseCost.ToString());

        // Setup the button
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // Check if the clicked entity object can transfer the item
        bool itemTransfered =
            clickedEntity.TryTransferItem(item, player, clickedEntity, isPlayerItem);
        
        if (itemTransfered)
        {
            // Do something if the item is transferred
            // Need to update the UI list

        }
    }
}
