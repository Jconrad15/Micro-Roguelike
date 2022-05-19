using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEntity : Entity
{
    public bool IsAIControlled { get; protected set; }

    public AIEntity(Tile t, EntityType type) : base(t, type)
    {
        IsAIControlled = true;
    }
}
