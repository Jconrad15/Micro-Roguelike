using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private Action<int, int> cbOnPlayerMoved;
    private Action cbOnInventoryToggled;

    private void OnEnable()
    {
        WorldGenerator worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.RegisterOnPlayerCreated(OnPlayerCreated);

        TurnController.Instance.RegisterOnStartPlayerTurn(OnStartTurn);
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
        // Set player as visible
        p.Visibility = VisibilityLevel.Visible;

        VisibilityChanger.UpdateTileVisibility(player);
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
            PlayerInputAction();

            playerMoved = TryPlayerInputMovement();

            yield return null;
        }

        cbOnPlayerMoved?.Invoke(player.X, player.Y);

        // Update visibilities based on player position
        VisibilityChanger.UpdateTileVisibility(player);
        VisibilityChanger.UpdateEntityVisibility(player);
        VisibilityChanger.UpdateFeatureVisibility(player);

        // Player's turn is done
        TurnController.Instance.NextTurn();
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

}