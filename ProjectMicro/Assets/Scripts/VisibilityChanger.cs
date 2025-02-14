using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VisibilityChanger
{
    public static void UpdateTileVisibility(Player player)
    {
        Tile[] mapData = AreaData.GetMapDataForCurrentType();
        
        for (int i = 0; i < mapData.Length; i++)
        {
            Tile t = mapData[i];
            if (Vector2.Distance(
                new Vector2(t.x, t.y),
                new Vector2(player.X, player.Y)) 
                < player.stats.VisibilityDistance)
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
        List<Entity> entities = AreaData.GetEntitiesForCurrentType();

        for (int i = 0; i < entities.Count; i++)
        {
            Entity e = entities[i];

            // Skip the player
            if (e.type == EntityType.Player) { continue; }

            if (Vector2.Distance(
                new Vector2(e.X, e.Y),
                new Vector2(player.X, player.Y)) 
                < player.stats.VisibilityDistance)
            {
                e.Visibility = VisibilityLevel.Visible;
            }
            else
            {
                // Visible changes to not visible for entities
                if (e.Visibility == VisibilityLevel.Visible)
                {
                    e.Visibility = VisibilityLevel.NotVisible;
                }
            }
        }
    }

    public static void UpdateFeatureVisibility(Player player)
    {
        List<Feature> features = AreaData.GetFeaturesForCurrentType();

        for (int i = 0; i < features.Count; i++)
        {
            Feature f = features[i];

            if (Vector2.Distance(
                new Vector2(f.T.x, f.T.y),
                new Vector2(player.X, player.Y)) 
                < player.stats.VisibilityDistance)
            {
                f.Visibility = VisibilityLevel.Visible;
            }
            else
            {
                // Visible changes to previously seen
                if (f.Visibility == VisibilityLevel.Visible)
                {
                    f.Visibility = VisibilityLevel.PreviouslySeen;
                }
            }
        }
    }

}
