using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject endArea;

    [SerializeField]
    private TextMeshProUGUI endText;

    private void Start()
    {
        WinLoseManager.Instance.RegisterOnPlayerLose(OnLose);
        WinLoseManager.Instance.RegisterOnPlayerWin(OnWin);

        Hide();
    }

    private void OnLose()
    {
        SetLoseText();
        Show();
    }

    private void SetLoseText()
    {
        endText.SetText("Game Over!");
    }

    private void OnWin()
    {
        SetVictoryText();
        Show();
    }

    private void SetVictoryText()
    {
        endText.SetText(
            "Congratulations! You have earned a rare merchant license!");
    }

    private void Show()
    {
        endArea.SetActive(true);
    }

    private void Hide()
    {
        endArea.SetActive(false);
    }
}
