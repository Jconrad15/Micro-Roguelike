using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Strong : Trait
{
    private readonly int inventorySizeAdjustment = 1;

    private readonly int cost = 1;
    public override int Cost => cost;

    private readonly string traitName = "Strong";
    public override string TraitName => traitName;

    public override void ApplyTrait(Entity entity)
    {
        entity.stats.AdjustInventorySize(inventorySizeAdjustment);
    }
}
