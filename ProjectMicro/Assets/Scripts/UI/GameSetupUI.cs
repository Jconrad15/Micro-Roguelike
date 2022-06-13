using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetupUI : MonoBehaviour
{
    [SerializeField]
    private GameObject gameSetupUIArea;

    private string currentName;

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

    public void InputCharacterName(string characterName)
    {
        currentName = characterName;
    }

}
