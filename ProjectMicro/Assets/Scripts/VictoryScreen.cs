using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject victoryArea;

    [SerializeField]
    private TextMeshProUGUI victoryText;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    public void Victory()
    {
        SetVictoryText();
        Show();
    }

    private void SetVictoryText()
    {
        victoryText.SetText(
            "Congratulations! You have earned a rare merchant license!");
    }

    private void Show()
    {
        victoryArea.SetActive(true);
    }

    private void Hide()
    {
        victoryArea.SetActive(false);
    }
}
