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

    private readonly int titleCost = 250;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().GetPlayer();
        player.RegisterOnLicenseChanged(OnPlayerLicenseChanged);
        SetLicenses();
    }

    private void OnEnable()
    {
        SetLicenses();
    }

    private void SetLicenses()
    {
        if (player == null) { return; }

        currentLicense.SetText(player.License.ToString());

        // Set next title
        if (player.License == Player.PlayerLicense.Traveller)
        {
            nextLicense.SetText(
                Player.PlayerLicense.Merchant.ToString() + " License");
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
            WinLoseManager.Instance.Win();
        }
    }

    private void OnPlayerLicenseChanged(Player.PlayerLicense newLicense)
    {

    }
}
