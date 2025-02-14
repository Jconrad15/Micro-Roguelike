using System;

[Serializable]
public abstract class Trait
{
    /*
    Adjustors are a flat value that is added or subtracted (usually int)
    Modifies are a multiplicative value (usually float)
    */

    public abstract int Cost { get; }
    public abstract string TraitName { get; }
    public abstract string Description { get; }
    public abstract void ApplyTrait(Entity entity);

}
