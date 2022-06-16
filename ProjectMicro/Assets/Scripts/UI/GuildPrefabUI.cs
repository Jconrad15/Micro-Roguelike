using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GuildPrefabUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI guildNameText;
    [SerializeField]
    private Toggle guildToggle;

    private Guild guild;

    public Toggle Setup(Guild guild, GuildToggleGroupUI guildToggleGroupUI)
    {
        this.guild = guild;
        // Set guild info
        guildNameText.SetText(guild.GuildName);

        guildToggle.onValueChanged.AddListener(
            (bool isSelected) =>
            {
                if (isSelected)
                {
                    guildToggleGroupUI.SelectGuild(guild);
                }
            });

        return guildToggle;
    }
    
}
