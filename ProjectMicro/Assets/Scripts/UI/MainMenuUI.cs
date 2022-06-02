using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void NewGameButton()
    {
        SceneChanger.Instance.SwitchToGameScene();
    }

    public void LoadGameButton()
    {
        SceneBus.Instance.SetIsLoadGame(true);
        SceneChanger.Instance.SwitchToGameScene();
    }
}
