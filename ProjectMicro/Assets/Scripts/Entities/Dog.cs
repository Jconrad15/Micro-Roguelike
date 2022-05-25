using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : AIEntity
{
    public Dog(Tile t, EntityType type, int startingMoney)
        : base(t, type, startingMoney)
    {
        EntityName = "dog";
    }

}
