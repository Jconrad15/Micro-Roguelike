using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitlePurchaser : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentTitle;
    [SerializeField]
    private TextMeshProUGUI nextTitle;
    [SerializeField]
    private TextMeshProUGUI costText;

    private Player player;

    private int titleCost = 20;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().GetPlayer();
        SetTitles();
    }

    private void OnEnable()
    {
        SetTitles();
    }

    private void SetTitles()
    {
        if (player == null) { return; }

        currentTitle.SetText(player.Title.ToString());

        // Set next title
        if (player.Title == Player.PlayerTitle.Traveller)
        {
            nextTitle.SetText(
                Player.PlayerTitle.Merchant.ToString() + " License");
        }

        costText.SetText("$" + titleCost.ToString());
    }

    public void PurchaseTitleButton()
    {
        //Try purchase
        bool isPurchased = player.TryPurchaseTitle(titleCost);
        if (isPurchased)
        {
            // If purchased
            Debug.Log("You win");
        }
    }

}
