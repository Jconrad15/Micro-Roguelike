using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : AIEntity
{
    public Animal(
        Tile t, EntityType type, int startingMoney,
        List<Trait> traits)
        : base(t, type, startingMoney, traits) 
    {
        EntityName = "animal";
    }
}
