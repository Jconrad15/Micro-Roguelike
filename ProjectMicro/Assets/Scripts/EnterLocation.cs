using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterLocation : MonoBehaviour
{
    void Update()
    {
        // If on world map
        if (AreaDataManager.Instance.CurrentMapType == MapType.World)
        {
            // If hit space bar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TryEnterLocation();
            }
        }
    }

    private void TryEnterLocation()
    {
        // Get player location
        Player player = FindObjectOfType<PlayerController>().GetPlayer();

        TileType tileType = player.T.Type;

        // Save world location for now
        WorldGenerator.Instance.SavePlayerWorldPosition(player.X, player.Y);

        // First need to destroy all current info
        DataLoader.ClearAllOldData();

        // Then load the location
        LocationGenerator.Instance.StartGenerateLocation(
            player.X, player.Y,
            tileType, player,
            player.T.TileFeature);
    }
}
