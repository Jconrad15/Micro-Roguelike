using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject escapeMenuArea;

    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (escapeMenuArea.activeSelf == true)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        escapeMenuArea.SetActive(true);
    }

    public void Hide()
    {
        escapeMenuArea.SetActive(false);
    }
}
