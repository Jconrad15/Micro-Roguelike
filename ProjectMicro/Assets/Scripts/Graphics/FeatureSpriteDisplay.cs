using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureSpriteDisplay : MonoBehaviour
{
    private GameObject featuresContainer;
    private Dictionary<Feature, GameObject> placedFeatures;
    private SpriteDatabase spriteDatabase;

    [SerializeField]
    private GameObject featurePrefab;

    [SerializeField]
    private VisibilityAlphaChanger visibilityAlphaChanger;

    private void Awake()
    {
        spriteDatabase = FindObjectOfType<SpriteDatabase>();
    }

    private void OnEnable()
    {
        featuresContainer = new GameObject("Features");
        featuresContainer.transform.parent = transform;
        placedFeatures = new Dictionary<Feature, GameObject>();
    }

    public void PlaceInitialFeature(Feature feature, int x, int y)
    {
        spriteDatabase.FeatureDatabase
            .TryGetValue(feature.type, out Sprite[] s);

        CreateFeatureGO(feature, x, y, s,
            out GameObject newFeature, out SpriteRenderer sr);

        // Visibility
        visibilityAlphaChanger.ChangeVisibilityAlpha(feature, sr);

        placedFeatures.Add(feature, newFeature);

        feature.RegisterOnVisibilityChanged(OnVisiblityChanged);
    }

    private void CreateFeatureGO(
        Feature feature, int x, int y, Sprite[] s,
        out GameObject newFeature, out SpriteRenderer sr)
    {
        newFeature =
            Instantiate(featurePrefab, featuresContainer.transform);
        newFeature.transform.position = new Vector2(x, y);

        sr = newFeature.GetComponent<SpriteRenderer>();
        int selectedSpriteIndex = DetermineSprite(feature);
        sr.sprite = s[selectedSpriteIndex];
    }

    private static int DetermineSprite(Feature feature)
    {
        return 0;
    }

    private void OnVisiblityChanged(Feature f)
    {
        if (placedFeatures.TryGetValue(f, out GameObject tile_GO))
        {
            visibilityAlphaChanger.ChangeVisibilityAlpha(
                f, tile_GO.GetComponent<SpriteRenderer>());
        }
        else
        {
            Debug.LogError("What Feature is this?" +
                "Not in entity-GO dictionary.");
        }
    }

    public void ClearAll()
    {
        List<GameObject> currentGOs = new List<GameObject>();

        foreach (GameObject go in placedFeatures.Values)
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
        foreach (Feature feature in placedFeatures.Keys)
        {
            feature.UnregisterOnVisibilityChanged(OnVisiblityChanged);
        }

        // Clear dictionary
        placedFeatures = new Dictionary<Feature, GameObject>();

    }
}
