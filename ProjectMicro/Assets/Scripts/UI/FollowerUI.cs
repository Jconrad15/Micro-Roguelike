using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private List<GameObject> createdFollowers;
    private bool isRegistered = false;

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);
        createdFollowers = new List<GameObject>();
    }

    private void OnPlayerCreated(Player p)
    {
        player = p;
        RegisterToNewFollowers();
    }

    private void RegisterToNewFollowers()
    {
        if (isRegistered == true) { return; }

        player.RegisterOnFollowerAdded(OnFollowerAdded);
        isRegistered = true;
        Debug.Log("RegisterOnFollowerAdded");
    }

    private void OnFollowerAdded(Follower f)
    {
        Debug.Log("OnFollowerAdded");
        CreateFollower(f.Entity);
    }

    private void CreateFollower(Entity entity)
    {
        // Create followers
        GameObject followerGO = Instantiate(followerPrefab);
        followerGO.transform.SetParent(followerContainer.transform);

        Debug.Log("Create Follower");

        FollowerPrefabUI followerPrefabUI =
            followerGO.GetComponent<FollowerPrefabUI>();

        followerPrefabUI.Setup(entity);

        createdFollowers.Add(followerGO);
    }

}
