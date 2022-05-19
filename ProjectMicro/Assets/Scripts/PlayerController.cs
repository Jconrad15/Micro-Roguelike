using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Player player;
    private float counterThreshold = 0.1f;
    private float counter = 0;

    public void Start()
    {
        WorldGenerator worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.RegisterOnPlayerCreated(OnPlayerCreated);
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
    }

    private void Update()
    {
        if (counter < counterThreshold) 
        {
            counter += Time.deltaTime;
            return;
        }

        // Exit loop if no player
        if (player == null) return;

        float hMovement = Input.GetAxisRaw("Horizontal");
        float vMovement = Input.GetAxisRaw("Vertical");

        // Move horizontal first
        if (Mathf.Abs(hMovement) == 1f)
        {
            Direction d = hMovement == 1 ? Direction.E : Direction.W;
            player.TryMove(d);
        }
        else if (Mathf.Abs(vMovement) == 1f)
        {
            Direction d = vMovement == 1 ? Direction.N: Direction.S;
            player.TryMove(d);
        }

        // Reset counter
        counter = 0;
    }

}