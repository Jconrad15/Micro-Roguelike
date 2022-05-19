using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;

    private Action<int, int> cbOnPlayerMoved; 

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

        UpdateVisibility();
    }

    private void OnStartTurn()
    {
        StartCoroutine(PlayerProcessing());
    }

    private IEnumerator PlayerProcessing()
    {
        bool playerMoved = false;

        while (playerMoved == false)
        {
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

            yield return null;
        }

        cbOnPlayerMoved?.Invoke(player.X, player.Y);

        // Change tile visibility
        UpdateVisibility();


        // Player's turn is done
        TurnController.Instance.NextTurn();
    }

    private void UpdateVisibility()
    {
        for (int i = 0; i < WorldData.Instance.MapData.Length; i++)
        {
            Tile t = WorldData.Instance.MapData[i];
            if (Vector2.Distance(
                new Vector2(t.x, t.y),
                new Vector2(player.X, player.Y)) < 5)
            {
                t.Visibility = VisibilityLevel.Visible;
            }
            else
            {
                // Visible changes to previously seen
                if (t.Visibility == VisibilityLevel.Visible)
                {
                    t.Visibility = VisibilityLevel.PreviouslySeen;
                }
            }
        }
    }

    public void RegisterOnPlayerMove(Action<int, int> callbackfunc)
    {
        cbOnPlayerMoved += callbackfunc;
    }

    public void UnregisterOnPlayerMove(Action<int, int> callbackfunc)
    {
        cbOnPlayerMoved -= callbackfunc;
    }

}