using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleViewer : MonoBehaviour
{
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        AreaDataManager.Instance.RegisterOnCurrentMapTypeChange(OnCurrentMapTypeChange);
        ShowTurtle();
    }

    private void OnCurrentMapTypeChange(MapType mapType)
    {
        if (mapType == MapType.World)
        {
            ShowTurtle();
        }
        else
        {
            HideTurtle();
        }
    }

    private void ShowTurtle()
    {
        Color c = sr.color;
        c.a = 1;
        sr.color = c;
    }

    private void HideTurtle()
    {
        Color c = sr.color;
        c.a = 0;
        sr.color = c;
    }
}
