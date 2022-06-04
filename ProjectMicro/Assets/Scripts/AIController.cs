using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private List<AIEntity> aiEntities = new List<AIEntity>();

    private void Awake()
    {
        AIEntityInstantiation.RegisterOnAIEntityCreated(OnAICreated);
    }

    private void OnEnable()
    {
        TurnController.Instance.RegisterOnStartAITurn(OnAITurn);
    }

    private void OnAICreated(AIEntity aiEntity)
    {
        // Try to nullify any pathfinding a loaded entity may have
        aiEntity.NullPathfinding();

        aiEntities.Add(aiEntity);

        if (aiEntity.T == null)
        {
            Debug.LogError("New entity's tile is null");
        }
    }

    private void OnAITurn()
    {
        StartCoroutine(AIProcessing()); 
    }

    public void ClearAll()
    {
        aiEntities = new List<AIEntity>();
    }

    private IEnumerator AIProcessing()
    {
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < aiEntities.Count; i++)
        {
            AIEntity aiEntity = aiEntities[i];

            // Does ai entity need a new path from pathfinding
            _ = aiEntity.TryDetermineNewDestination();

            bool moved = aiEntity.TryMove(aiEntity.NextTile);
            if (moved) { aiEntity.Moved(); }
        }

        TurnController.Instance.NextTurn();
    }
}
