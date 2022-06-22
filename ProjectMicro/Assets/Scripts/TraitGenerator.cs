using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TraitGenerator
{
    private static readonly Trait strong = new Strong();
    private static readonly Trait weak = new Weak();
    private static readonly Trait caravanDriver = new CaravanDriver();
    private static readonly Trait nearsighted = new Nearsighted();

    public static List<Trait> GenerateTraits(int traitSeed)
    {
        Random.State oldState = Random.state;
        Random.InitState(traitSeed);

        List<Trait> traits = new List<Trait>();

        // While no traits
        while (traits.Count == 0)
        {
            SelectTraits(traits);
        }

        Random.state = oldState;
        return traits;
    }

    private static void SelectTraits(List<Trait> traits)
    {
        if (Random.value > 0.8)
        {
            if (Random.value > 0.5f)
            {
                traits.Add(strong);
            }
            else
            {
                traits.Add(weak);
            }
        }

        if (Random.value > 0.8f)
        {
            traits.Add(nearsighted);
        }

        if (Random.value > 0.8f)
        {
            traits.Add(caravanDriver);
        }
    }
}
