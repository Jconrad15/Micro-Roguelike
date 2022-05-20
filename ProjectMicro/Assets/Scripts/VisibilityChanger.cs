using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisibilityChanger
{
    public static void UpdateTileVisibility(Player player)
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

    public static void UpdateEntityVisibility(Player player)
    {
        List<Entity> entities = WorldData.Instance.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            Entity e = entities[i];

            // Skip the player
            if (e.type == EntityType.Player) { continue; }

            if (Vector2.Distance(
                new Vector2(e.X, e.Y),
                new Vector2(player.X, player.Y)) < 5)
            {
                e.Visibility = VisibilityLevel.Visible;
            }
            else
            {
                // Visible changes to previously seen
                if (e.Visibility == VisibilityLevel.Visible)
                {
                    e.Visibility = VisibilityLevel.NotVisible;
                }
            }
        }
    }

}
