public class Nearsighted : Trait
{
    private readonly int visionDistanceAdjustment = -1;

    private readonly int cost = 1;
    public override int Cost => cost;

    private readonly string traitName = "Nearsighted";
    public override string TraitName => traitName;

    private readonly string description = "Vision Distance -1";
    public override string Description => description;

    public override void ApplyTrait(Entity entity)
    {
        entity.stats.AdjustVisibilityDistance(visionDistanceAdjustment);
    }
}
