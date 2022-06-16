using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuildToggleGroupUI : MonoBehaviour
{
    [SerializeField]
    private GameObject guildPrefab;

    private List<GameObject> guildGOs = new List<GameObject>();

    private ToggleGroup guildToggleGroup;
    private GameSetupUI gameSetupUI;

    private List<GuildPrefabUI> guildPrefabUIs = new List<GuildPrefabUI>();

    private void OnEnable()
    {
        guildToggleGroup = GetComponent<ToggleGroup>();
    }

    public void DisplayGuilds(Guild[] guilds, GameSetupUI gameSetupUI)
    {
        this.gameSetupUI = gameSetupUI;
        ClearAll();

        Toggle[] toggles = new Toggle[guilds.Length];

        for (int i = 0; i < guilds.Length; i++)
        {
            Debug.Log("CreateGuild");
            // Show each guild
            Instantiate(guildPrefab, gameObject.transform);

            GuildPrefabUI guildPrefabUI =
                guildPrefab.GetComponent<GuildPrefabUI>();
            
            Toggle toggle = guildPrefabUI.SetGuild(guilds[i], SelectGuild);

            toggle.group = guildToggleGroup;

            guildPrefabUIs.Add(guildPrefabUI);
        }

    }

    private void ClearAll()
    {
        for (int i = 0; i < guildGOs.Count; i++)
        {
            Destroy(guildGOs[i]);
        }

        guildGOs.Clear();
    }

    public void SelectGuild(Guild selectedGuild)
    {
        Debug.Log("SelectGuild");
        gameSetupUI.SelectedGuild(selectedGuild);
    }

    private void OnDestroy()
    {
        foreach(GuildPrefabUI guildPrefabUI in guildPrefabUIs)
        {
            guildPrefabUI.UnregisterOnGuildSelected(SelectGuild);
        }
        guildPrefabUIs.Clear();
    }

}
