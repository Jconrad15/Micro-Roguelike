using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Trait
{
    /*
    Adjustors are a flat value that is added or subtracted (usually int)
    Modifies are a multiplicative value (usually float)
    */

    public abstract int Cost { get; }
    public abstract string TraitName { get; }

    public abstract void ApplyTrait(Entity entity);



}
