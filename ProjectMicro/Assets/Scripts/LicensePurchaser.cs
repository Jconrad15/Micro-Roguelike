using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LicensePurchaser : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentLicense;
    [SerializeField]
    private TextMeshProUGUI nextLicense;
    [SerializeField]
    private TextMeshProUGUI costText;

    private Player player;

    private readonly int titleCost = 20;

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

        currentLicense.SetText(player.Title.ToString());

        // Set next title
        if (player.Title == Player.PlayerTitle.Traveller)
        {
            nextLicense.SetText(
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
            FindObjectOfType<VictoryScreen>().Victory();
        }
    }

}
