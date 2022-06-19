using System.Collections.Generic;
using UnityEngine;

public class TraitSelectionManagerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject TraitIconPrefab;

    [SerializeField]
    private GameObject traitPrefabArea;

    private List<GameObject> traitGOs;

    //private int selectionPoints;

    private List<Trait> traits = new List<Trait>();

    public List<Trait> GetTraits()
    {
        // Create blank trait list if no traits
        if (traits == null)
        {
            traits = new List<Trait>();
        }

        return traits;
    }

    public void Display()
    {
        ClearAll();
        for (int i = 0; i < traits.Count; i++)
        {
            GameObject traitGO =
                Instantiate(TraitIconPrefab, traitPrefabArea.transform);

            traitGO.GetComponent<TraitIconUI>().Setup(traits[i]);
            traitGOs.Add(traitGO);
        }
    }

    private void ClearAll()
    {
        if (traitGOs == null)
        { 
            traitGOs = new List<GameObject>();
            return;
        }

        if (traitGOs.Count == 0) { return; }

        foreach (GameObject traitGO in traitGOs)
        {
            Destroy(traitGO);
        }
        traitGOs = new List<GameObject>();
    }

    internal List<Trait> GenerateTraits(int traitSeed)
    {
        traits = TraitGenerator.GenerateTraits(traitSeed);
        return traits;
    }
    /*
   public bool TrySelectTrait(Trait selectedTrait)
   {
       if (selectionPoints < selectedTrait.Cost) { return false; }

       if (selectedTrait == null) { return false; }

       if (traits.Contains(selectedTrait)) { return false; }

       AddTrait(selectedTrait);
       return true;
   }

   private void AddTrait(Trait selectedTrait)
   {
       traits.Add(selectedTrait);
       selectionPoints -= selectedTrait.Cost;
   }
*/

}
