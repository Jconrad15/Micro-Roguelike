using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FollowerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject followerArea;
    [SerializeField]
    private GameObject followerContainer;

    [SerializeField]
    private GameObject followerPrefab;

    private Player player;

    private GameObject[] createdFollowers;

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
        //player.RegisterOnFollowerAdded(OnFollowerAdded);
    }

    public void Show(Entity clickedEntity)
    {
        UIModality.Instance.IsDialogueOpen = true;

        //CreateFollower(clickedEntity);

        // Show player and trader money
        followerArea.SetActive(true);
    }

/*    private void OnFollowerAdded(Follower f)
    {

    }

    private void CreateFollower(Entity clickedEntity)
    {
        // Create followers
        createdTraderItems = new GameObject[traderItems.Count];
        for (int i = 0; i < traderItems.Count; i++)
        {
            GameObject newItem_GO =
                Instantiate(followerPrefab, followerContainer.transform);
            TradeItemUI tradeItemUI = newItem_GO.GetComponent<TradeItemUI>();
            tradeItemUI.Setup(traderItems[i], player, clickedEntity, false);
            tradeItemUI.RegisterOnItemTransfered(OnItemTransfered);
            createdTraderItems[i] = newItem_GO;
        }
    }*/

}
