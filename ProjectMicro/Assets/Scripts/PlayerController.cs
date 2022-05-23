using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private Action<int, int> cbOnPlayerMoved;
    private Action cbOnInventoryToggled;
    //private bool isPlayerTurn = false;

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
        //isPlayerTurn = true;
        StartCoroutine(PlayerProcessing());
    }

    private IEnumerator PlayerProcessing()
    {
        // Leave loop and end turn when player moves
        bool playerMoved = false;
        while (playerMoved == false)
        {
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
        //isPlayerTurn = false;
    }

    private void Update()
    {
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
        // If dialogue is open, no movement allowed
        if (UIModality.Instance.IsDialogueOpen) { return false; }

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

}