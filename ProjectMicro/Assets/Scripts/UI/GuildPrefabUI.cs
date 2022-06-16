using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuildPrefabUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI guildNameText;

    public Toggle SetGuild(Guild guild)
    {
        // Set guild info
        guildNameText.SetText(guild.GuildName);

        Toggle toggle = GetComponent<Toggle>();
        return toggle;
    }

}
