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

        List<Toggle> toggles = new List<Toggle>();

        foreach (Guild guild in guilds)
        {
            Debug.Log("CreateGuild");
            // Show each guild
            Instantiate(guildPrefab, gameObject.transform);

            Toggle t =
                guildPrefab.GetComponent<GuildPrefabUI>().SetGuild(guild);

            t.group = guildToggleGroup;
            toggles.Add(t);
        }

        guildToggleGroup.SetAllTogglesOff();

        foreach (Toggle toggle in toggles)
        {
            guildToggleGroup.RegisterToggle(toggle);
            toggle.onValueChanged.AddListener(
                delegate
                {
                    // TODO this registration
                });
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

    private void SelectGuild(Guild selectedGuild)
    {
        gameSetupUI.SelectedGuild(selectedGuild);
    }

}
