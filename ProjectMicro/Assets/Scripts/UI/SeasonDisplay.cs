using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SeasonDisplay : MonoBehaviour
{
    [SerializeField]
    private SeasonManager seasonManager;
    [SerializeField]
    private TextMeshProUGUI seasonText;
    [SerializeField]
    private TextMeshProUGUI dayText;

    private void Start()
    {
        seasonManager.RegisterOnDayChanged(OnDayChanged);
        seasonManager.RegisterOnSeasonChanged(OnSeasonChanged);
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
        if (seasonManager != null)
        {
            seasonManager.UnregisterOnDayChanged(OnDayChanged);
            seasonManager.UnregisterOnSeasonChanged(OnSeasonChanged);
        }
    }
}
