using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : AIEntity
{
    public Animal(
        Tile t, EntityType type, int startingMoney,
        List<Attribute> attributes)
        : base(t, type, startingMoney, attributes) 
    {
        EntityName = "animal";
    }
}
