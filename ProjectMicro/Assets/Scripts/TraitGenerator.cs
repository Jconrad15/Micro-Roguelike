using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TraitGenerator
{
    private static readonly Trait strong = new Strong();
    private static readonly Trait weak = new Weak();

    public static List<Trait> GenerateTraits(int traitSeed)
    {
        Random.State oldState = Random.state;
        Random.InitState(traitSeed);

        List<Trait> traits = new List<Trait>();

        if (Random.value > 0.5f)
        {
            traits.Add(strong);
        }
        else
        {
            traits.Add(weak);
        }

        Random.state = oldState;
        return traits;
    }

}
