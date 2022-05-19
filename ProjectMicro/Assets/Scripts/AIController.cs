using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private List<AIEntity> aiEntities = new List<AIEntity>();

    private void Start()
    {
        TurnController.Instance.RegisterOnStartAITurn(OnAITurn);

        WorldGenerator worldGenerator = FindObjectOfType<WorldGenerator>();
        worldGenerator.RegisterOnAIEntityCreated(OnAICreated);
    }

    private void OnAICreated(AIEntity aiEntity)
    {
        aiEntities.Add(aiEntity);
    }

    private void OnAITurn()
    {
        StartCoroutine(AIProcessing()); 
    }

    private IEnumerator AIProcessing()
    {
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < aiEntities.Count; i++)
        {
            aiEntities[i].TryMove(Utility.GetRandomEnum<Direction>());
        }

        TurnController.Instance.NextTurn();
    }
}
