using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeasonDisplay : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI seasonText;
    [SerializeField]
    private TextMeshProUGUI dayText;

    private void Start()
    {
        SeasonManager.Instance.RegisterOnDayChanged(OnDayChanged);
        SeasonManager.Instance.RegisterOnSeasonChanged(OnSeasonChanged);

        OnDayChanged(SeasonManager.Instance.Day);
        OnSeasonChanged(SeasonManager.Instance.CurrentSeason);
    }

    private void OnDayChanged(int day)
    {
        string text = day.ToString();
        if (day % 20 == 1)
        {
            text += "st";
        }
        else if (day % 20 == 2)
        {
            text += "nd";
        }
        else if (day % 20 == 3)
        {
            text += "rd";
        }
        else
        {
            text += "th";
        }

        dayText.SetText(text);
    }

    private void OnSeasonChanged(Season season)
    {
        seasonText.SetText(season.ToString());
    }

    private void OnDestroy()
    {
        SeasonManager.Instance.UnregisterOnDayChanged(OnDayChanged);
        SeasonManager.Instance.UnregisterOnSeasonChanged(OnSeasonChanged);
    }
}
