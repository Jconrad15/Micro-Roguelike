using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton indicating if a UI is open
/// </summary>
public class UIModality : MonoBehaviour
{
    private Action cbOnEscapeMenuOpened;

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

    private bool isEscapeMenuOpen = false;
    public bool IsEscapeMenuOpen 
    { 
        get => isEscapeMenuOpen;
        set
        {
            isEscapeMenuOpen = value;
            cbOnEscapeMenuOpened?.Invoke();
        }    
    }

    public bool IsDialogueOpen { get; set; } = false;
    public bool IsInventoryOpen { get; private set; } = false;

    public void ToggleInventoryOpen()
    {
        IsInventoryOpen = !IsInventoryOpen;
    }

    public void RegisterOnEscapeMenuOpened(Action callbackfunc)
    {
        cbOnEscapeMenuOpened += callbackfunc;
    }

    public void UnregisterOnEscapeMenuOpened(Action callbackfunc)
    {
        cbOnEscapeMenuOpened -= callbackfunc;
    }
}
