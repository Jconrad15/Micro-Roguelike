using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{

    public void Show(GameObject go)
    {
        go.SetActive(true);
    }

    public void Hide(GameObject go)
    {
        go.SetActive(false);
    }

}
