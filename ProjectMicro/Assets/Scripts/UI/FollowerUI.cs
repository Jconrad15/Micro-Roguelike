using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    SpriteDatabase spriteDatabase;

    private void OnEnable()
    {
        PlayerInstantiation.RegisterOnPlayerCreated(OnPlayerCreated);
        createdFollowers = new List<GameObject>();

        spriteDatabase = FindObjectOfType<SpriteDatabase>();
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
    }

    private void OnFollowerAdded(Follower f)
    {
        CreateFollower(f.Entity);
    }

    private void CreateFollower(Entity entity)
    {
        // Create followers
        GameObject followerGO = Instantiate(followerPrefab);
        followerGO.transform.SetParent(followerContainer.transform);

        FollowerPrefabUI followerPrefabUI =
            followerGO.GetComponent<FollowerPrefabUI>();

        Image image = followerGO.GetComponentInChildren<Image>();

        spriteDatabase.EntityDatabase
            .TryGetValue(entity.EntityName, out Sprite[] s);
        image.sprite = s[0];

        followerPrefabUI.Setup(entity);

        createdFollowers.Add(followerGO);
    }

}
