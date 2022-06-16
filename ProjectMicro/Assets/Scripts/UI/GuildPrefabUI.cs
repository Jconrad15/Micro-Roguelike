using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GuildPrefabUI : MonoBehaviour, IPointerUpHandler
{
    private Action<Guild> cbOnGuildSelected;

    [SerializeField]
    private TextMeshProUGUI guildNameText;
    [SerializeField]
    private Toggle guildToggle;

    private Guild guild;

    public Toggle SetGuild(Guild guild, Action<Guild> callbackfunc)
    {
        this.guild = guild;
        // Set guild info
        guildNameText.SetText(guild.GuildName);

        RegisterOnGuildSelected(callbackfunc);
        return guildToggle;
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnMouseUp");
        cbOnGuildSelected?.Invoke(guild);
    }

    public void RegisterOnGuildSelected(Action<Guild> callbackfunc)
    {
        cbOnGuildSelected += callbackfunc;

        Delegate[] test = cbOnGuildSelected.GetInvocationList();
        Debug.Log(test.Length);
    }

    public void UnregisterOnGuildSelected(Action<Guild> callbackfunc)
    {
        cbOnGuildSelected -= callbackfunc;
    }
}
