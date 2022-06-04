using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager
{
    public List<Entity> Followers { get; private set; }

    public FollowerManager()
    {
        Followers = new List<Entity>();
    }

    public void AddFollower(Entity e)
    {
        if (e == null) { return; }
        Followers.Add(e);
    }

}
