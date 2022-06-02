using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SwitchToGameScene()
    {
        StartCoroutine(AsyncLoadScene(1));
    }

    private IEnumerator AsyncLoadScene(int buildIndex)
    {
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        // Wait until the asynchronous scene fully loads
        while (asyncLoad.isDone == false)
        {
            yield return null;
        }
    }
}
