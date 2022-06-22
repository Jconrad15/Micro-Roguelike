using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private Action<int, int> cbOnPlayerMoved;
    private Action<Player> cbOnPlayerGoToExitTile;
    private Action cbOnInventoryToggled;

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);
        TurnController.Instance.RegisterOnStartPlayerTurn(OnStartTurn);
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
        // Set player as visible
        p.Visibility = VisibilityLevel.Visible;

        VisibilityChanger.UpdateTileVisibility(player);
        VisibilityChanger.UpdateEntityVisibility(player);
        VisibilityChanger.UpdateFeatureVisibility(player);
    }

    private void OnStartTurn()
    {
        StartCoroutine(PlayerProcessing());
    }

    private IEnumerator PlayerProcessing()
    {
        // Leave loop and end turn when player moves
        bool playerMoved = false;
        while (playerMoved == false)
        {
            playerMoved = TryPlayerInputMovement();

            yield return new WaitForEndOfFrame();
        }

        cbOnPlayerMoved?.Invoke(player.X, player.Y);

        // Check if player entered an exit tile
        if (AreaDataManager.Instance.CurrentMapType == MapType.Location)
        {
            if (player.T.TileFeature != null)
            {
                if (player.T.TileFeature.type == FeatureType.ExitLocation)
                {
                    cbOnPlayerGoToExitTile?.Invoke(player);
                }
            }
        }

        // Update visibilities based on player position
        VisibilityChanger.UpdateTileVisibility(player);
        VisibilityChanger.UpdateEntityVisibility(player);
        VisibilityChanger.UpdateFeatureVisibility(player);

        // Player's turn is done
        TurnController.Instance.NextTurn();
    }

    private void Update()
    {
        if (player == null) { return; }

        PlayerInputAction();
    }

    private void PlayerInputAction()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            // Toggle the inventory 
            cbOnInventoryToggled?.Invoke();
        }
    }

    /// <summary>
    /// Receives input and determines player movement
    /// </summary>
    /// <returns></returns>
    private bool TryPlayerInputMovement()
    {
        // If dialogue or escape menu is open, no movement allowed
        if (UIModality.Instance.IsDialogueOpen ||
            UIModality.Instance.IsEscapeMenuOpen ) 
        { 
            return false; 
        }

        // If game is done, no movement allowed
        if (WinLoseManager.Instance.GameIsDone == true) { return false; }

        bool playerMoved = false;

        float hMovement = Input.GetAxisRaw("Horizontal");
        float vMovement = Input.GetAxisRaw("Vertical");

        // Move horizontal first
        if (Mathf.Abs(hMovement) == 1f)
        {
            Direction d = hMovement == 1 ? Direction.E : Direction.W;
            playerMoved = player.TryMove(d);
        }
        else if (Mathf.Abs(vMovement) == 1f)
        {
            Direction d = vMovement == 1 ? Direction.N : Direction.S;
            playerMoved = player.TryMove(d);
        }

        return playerMoved;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public void RegisterOnPlayerMove(Action<int, int> callbackfunc)
    {
        cbOnPlayerMoved += callbackfunc;
    }

    public void UnregisterOnPlayerMove(Action<int, int> callbackfunc)
    {
        cbOnPlayerMoved -= callbackfunc;
    }

    public void RegisterOnInventoryToggled(Action callbackfunc)
    {
        cbOnInventoryToggled += callbackfunc;
    }

    public void UnregisterOnInventoryToggled(Action callbackfunc)
    {
        cbOnInventoryToggled -= callbackfunc;
    }

    public void RegisterOnPlayerGoToExitTile(Action<Player> callbackfunc)
    {
        cbOnPlayerGoToExitTile += callbackfunc;
    }

    public void UnregisterOnPlayerGoToExitTile(Action<Player> callbackfunc)
    {
        cbOnPlayerGoToExitTile -= callbackfunc;
    }
}