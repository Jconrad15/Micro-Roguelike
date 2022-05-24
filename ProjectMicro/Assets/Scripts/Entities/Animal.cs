using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : AIEntity
{
    public Animal(Tile t, EntityType type) : base(t, type) 
    {
        entityName = "animal";
    }
}
