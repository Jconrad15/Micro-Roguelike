using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weak : Trait
{
    private readonly int inventorySizeAdjustment = -1;

    private readonly int cost = 1;
    public override int Cost => cost;

    private readonly string traitName = "Weak";
    public override string TraitName => traitName;

    public override void ApplyTrait(Entity entity)
    {
        entity.EditInventorySize(inventorySizeAdjustment);
    }
}
