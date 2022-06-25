using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject dustPrefab;

    private List<GameObject> spawnedVFX = new List<GameObject>();

    private Tile[] mapData;

    private float shuffleTimerThreshold = 10f;
    private float timer = 0f;

    private void OnEnable()
    {
        FindObjectOfType<WorldGenerator>()
            .RegisterOnWorldCreated(OnAreaCreated);
        FindObjectOfType<LocationGenerator>()
            .RegisterOnLocationCreated(OnAreaCreated);
    }

    /// <summary>
    /// When the location changes, get the new mapdata.
    /// </summary>
    private void OnAreaCreated()
    {
        ClearAll();
        mapData = AreaData.GetMapDataForCurrentType();
        PlaceVFX();
    }

    private void Update()
    {
        if (timer >= shuffleTimerThreshold)
        {
            ShuffleVFX();
            timer = 0;
            return;
        }

        timer += Time.deltaTime;
    }

    private void PlaceVFX()
    {
        // Place dust VFX

        // Determine locations
        int[] selectedTileIndices = new int[Random.Range(10, 20)];
        
        // TODO: Make computationally efficent
        for (int i = 0; i < selectedTileIndices.Length; i++)
        {
            bool isSelected = false;
            while (isSelected == false)
            {
                int selectedIndex = Random.Range(0, mapData.Length);
                
                // Check if selected index is in selected tiles array
                if (System.Array.IndexOf(
                    selectedTileIndices, selectedIndex) == -1)
                {
                    // Is not included
                    selectedTileIndices[i] = selectedIndex;

                    // Get vector position
                    Vector3 pos = new Vector3(
                        mapData[selectedIndex].x,
                        mapData[selectedIndex].y,
                        0);

                    // Create gameobject
                    GameObject newVFX = Instantiate(dustPrefab);
                    newVFX.name = "Dust " + i.ToString();
                    newVFX.transform.SetParent(transform);
                    spawnedVFX.Add(newVFX);

                    newVFX.transform.position = pos;
                    isSelected = true;
                }
            }
        }
    }

    private void ShuffleVFX()
    {
        if (spawnedVFX.Count == 0) { return; }

        int[] selectedTileIndices = new int[spawnedVFX.Count];

        for (int i = 0; i < spawnedVFX.Count; i++)
        {
            bool isSelected = false;
            while (isSelected == false)
            {
                int selectedIndex = Random.Range(0, mapData.Length);

                // Check if selected index is in selected tiles array
                if (System.Array.IndexOf(
                    selectedTileIndices, selectedIndex) == -1)
                {
                    // Is not included
                    selectedTileIndices[i] = selectedIndex;

                    // Get vector position
                    Vector3 pos = new Vector3(
                        mapData[selectedIndex].x,
                        mapData[selectedIndex].y,
                        0);

                    // Move gameobject
                    GameObject newVFX = spawnedVFX[i];
                    newVFX.name = "Dust " + i.ToString();
                    newVFX.transform.SetParent(transform);

                    newVFX.transform.position = pos;
                    isSelected = true;
                }
            }
        }
    }

    private void ClearAll()
    {
        for (int i = 0; i < spawnedVFX.Count; i++)
        {
            Destroy(spawnedVFX[i]);
        }
        spawnedVFX = new List<GameObject>();
    }

    private void OnDestroy()
    {
        WorldGenerator wg = WorldGenerator.Instance;
        if (wg != null)
        {
            wg.UnregisterOnWorldCreated(OnAreaCreated);
        }

        LocationGenerator lg = LocationGenerator.Instance;
        if (lg != null)
        {
            lg.UnregisterOnLocationCreated(OnAreaCreated);
        }
    }
}
