using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    private PlayerController playerController;

    [SerializeField]
    private GameObject dialogueArea;

    private void Start()
    {
        FindObjectOfType<WorldGenerator>().RegisterOnWorldCreated(OnWorldCreated);
    }

    private void OnWorldCreated()
    {
        // Register to all traders
        List<Entity> entities = WorldData.Instance.Entities;
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].RegisterOnTraderClicked(OnTraderClicked);
        }

        // Start hidden
        Hide();
    }

    private void OnTraderClicked(Entity entity)
    {
        Show();
    }

    public void Show()
    {
        UIModality.Instance.IsDialogueOpen = true;
        dialogueArea.SetActive(true);
    }

    public void Hide()
    {
        UIModality.Instance.IsDialogueOpen = false;
        dialogueArea.SetActive(false);
    }

}
