using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower
{
    public Entity Entity { get; private set; }

    public Follower(Entity entity)
    {
        Entity = entity;
    }


}
