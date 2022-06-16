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

    private void OnEnable()
    {
        guildToggleGroup = GetComponent<ToggleGroup>();
    }

    public void DisplayGuilds(Guild[] guilds, GameSetupUI gameSetupUI)
    {
        this.gameSetupUI = gameSetupUI;
        ClearAll();

        for (int i = 0; i < guilds.Length; i++)
        {
            // Show each guild
            GameObject guildGO =
                Instantiate(guildPrefab, gameObject.transform);

            GuildPrefabUI guildPrefabUI =
                guildGO.GetComponent<GuildPrefabUI>();
            
            Toggle toggle = guildPrefabUI.Setup(guilds[i], this);
            toggle.group = guildToggleGroup;

            guildGOs.Add(guildGO);
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
        gameSetupUI.SelectedGuild(selectedGuild);
    }

}
