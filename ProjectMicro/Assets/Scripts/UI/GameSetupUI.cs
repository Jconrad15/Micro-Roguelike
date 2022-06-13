using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupUI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameSetupUIArea;

    public void Show()
    {
        gameSetupUIArea.SetActive(true);
    }

    public void Hide()
    {
        gameSetupUIArea.SetActive(false);
    }

    public void StartButton()
    {
        GameInitializer.Instance.InitializeGame();
        Hide();
    }

}
