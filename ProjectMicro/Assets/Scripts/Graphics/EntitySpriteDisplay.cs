using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpriteDisplay : MonoBehaviour
{
    private GameObject entitiesContainer;
    private Dictionary<Entity, GameObject> placedEntities;
    private SpriteDatabase spriteDatabase;

    [SerializeField]
    private GameObject entitiesPrefab;

    [SerializeField]
    private VisibilityAlphaChanger visibilityAlphaChanger;

    private readonly float moveSpeed = 8f;

    private void Awake()
    {
        spriteDatabase = FindObjectOfType<SpriteDatabase>();
    }

    private void OnEnable()
    {
        entitiesContainer = new GameObject("Entities");
        entitiesContainer.transform.parent = transform;
        placedEntities = new Dictionary<Entity, GameObject>();
    }

    public void PlaceInitialEntity(Entity entity, int x, int y)
    {
        spriteDatabase.EntityDatabase
            .TryGetValue(entity.EntityName, out Sprite[] s);

        CreateEntityGO(entity, x, y, s,
            out GameObject newEntity_GO, out SpriteRenderer sr);

        // Visibility
        visibilityAlphaChanger.ChangeVisibilityAlpha(entity, sr);

        placedEntities.Add(entity, newEntity_GO);

        entity.RegisterOnMove(OnEntityMove);
        entity.RegisterOnVisibilityChanged(OnVisiblityChanged);

        // If entity is an AIEntity, register to on remove
        if (Utility.IsSameOrSubclass(typeof(AIEntity), entity.GetType()))
        {
            AIEntity aiEntity = (AIEntity)entity;
            aiEntity.RegisterOnAIEntityRemoved(OnAIEntityRemoved);
        }
    }

    private void CreateEntityGO(
        Entity entity, int x, int y, Sprite[] s,
        out GameObject newEntity_GO, out SpriteRenderer sr)
    {
        newEntity_GO =
            Instantiate(entitiesPrefab, entitiesContainer.transform);
        newEntity_GO.transform.position = new Vector2(x, y);

        sr = newEntity_GO.GetComponent<SpriteRenderer>();
        int selectedSpriteIndex = DetermineSprite(entity);
        sr.sprite = s[selectedSpriteIndex];

        // Set entity in the entity prefab components
        newEntity_GO.GetComponent<EntityClicker>()
            .SetEntity(entity);
        newEntity_GO.GetComponent<EntityMouseOver>()
            .SetEntity(entity);
        newEntity_GO.GetComponent<EntityNotificationGenerator>()
            .SetEntity(entity);
    }

    private static int DetermineSprite(Entity entity)
    {
        int selectedSpriteIndex;
        if (entity.type == EntityType.Player ||
            entity.GetType() == typeof(Merchant))
        {
            if (entity.T.Type == TileType.Water)
            {
                selectedSpriteIndex = 1;
            }
            else
            {
                selectedSpriteIndex = 0;
            }
        }
        else
        {
            selectedSpriteIndex = 0;
        }

        return selectedSpriteIndex;
    }

    private void OnEntityMove(Entity entity, Vector2 startPos)
    {
        if (placedEntities.TryGetValue(entity, out GameObject entity_GO))
        {
            // See if the sprite needs to change on move
            SwitchSprite(entity, entity_GO);

            StartCoroutine(SmoothEntityMove(entity, entity_GO, startPos));
        }
        else
        {
            Debug.Log("The entity's gameObject is missing");
        }
    }

    private void SwitchSprite(Entity entity, GameObject entity_GO)
    {
        spriteDatabase.EntityDatabase
            .TryGetValue(entity.EntityName, out Sprite[] s);

        int selectedSpriteIndex = DetermineSprite(entity);

        entity_GO.GetComponent<SpriteRenderer>().sprite =
            s[selectedSpriteIndex];
    }

    private void OnVisiblityChanged(Entity e)
    {
        if (placedEntities.TryGetValue(e, out GameObject tile_GO))
        {
            visibilityAlphaChanger.ChangeVisibilityAlpha(
                e, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What Entity is this?" +
                "Not in entity-GO dictionary.");
        }
    }

    private IEnumerator SmoothEntityMove(
    Entity entity, GameObject entity_GO, Vector2 startPos)
    {
        Vector2 destinationLocation = new Vector2(entity.X, entity.Y);
        Vector2 currentLocation = startPos;

        while (Vector3.Distance(destinationLocation, currentLocation)
            > 0.001f)
        {
            float step = moveSpeed * Time.deltaTime;
            currentLocation = Vector2.MoveTowards(
                currentLocation, destinationLocation, step);

            entity_GO.transform.position = currentLocation;
            yield return new WaitForEndOfFrame();
        }

        entity_GO.transform.position = destinationLocation;
    }

    public void ClearAll()
    {
        List<GameObject> currentGOs = new List<GameObject>();

        foreach (GameObject go in placedEntities.Values)
        {
            currentGOs.Add(go);
        }

        // Destroy all placed gameobjects
        foreach (GameObject go in currentGOs)
        {
            Destroy(go);
        }
        StopAllCoroutines();

        // Unregister from each object
        foreach (Entity entity in placedEntities.Keys)
        {
            entity.UnregisterOnMove(OnEntityMove);
            entity.UnregisterOnVisibilityChanged(OnVisiblityChanged);
        }

        // Clear dictionary
        placedEntities = new Dictionary<Entity, GameObject>();
    }

    private void OnAIEntityRemoved(AIEntity aiEntity)
    {
        if (aiEntity == null) 
        {
            Debug.Log("EntitySpriteDisplay: null aiEntity");
            return; 
        }

        if (placedEntities.TryGetValue(aiEntity, out GameObject aiEntity_GO))
        {
            Destroy(aiEntity_GO);
            placedEntities.Remove(aiEntity);
            Debug.Log("Destroy aiEntity sprite GO");
        }
    }
}
