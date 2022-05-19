using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TurnController.Instance.RegisterOnStartAITurn(OnAITurn);
    }

    private void OnAITurn()
    {
        StartCoroutine(AIProcessing()); 
    }

    private IEnumerator AIProcessing()
    {
        yield return new WaitForSeconds(0.1f);

        TurnController.Instance.NextTurn();
    }
}
