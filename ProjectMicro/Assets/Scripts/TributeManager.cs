using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton tracking the player's tribute.
/// </summary>
public class TributeManager : MonoBehaviour
{
    [SerializeField]
    private SeasonManager seasonManager;
    private PlayerController playerController;

    // Make singleton
    public static TributeManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        seasonManager.RegisterOnSeasonChanged(OnSeasonChanged);

        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnSeasonChanged(Season newSeason)
    {
        Player player = playerController.GetPlayer();
        bool tributePaid =
            player.TryPayTribute(GetTributeRatePerSeason(player.License));

        if (tributePaid == false)
        {
            // Player loses if they cannot pay their tribute
            WinLoseManager.Instance.Lose();
        }
    }
    public int GetTributeRatePerSeason(Player.PlayerLicense title)
    {
        if (title == Player.PlayerLicense.Traveller)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }

}
