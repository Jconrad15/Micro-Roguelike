using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton indicating if a UI is open
/// </summary>
public class UIModality : MonoBehaviour
{
    public static UIModality Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public bool IsInventoryOpen { get; private set; } = false;

    private void Start()
    {
        FindObjectOfType<PlayerController>()
            .RegisterOnInventoryToggled(OnInventoryToggled);
    }

    private void OnInventoryToggled()
    {
        IsInventoryOpen = !IsInventoryOpen;
    }





}
