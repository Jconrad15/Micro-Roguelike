using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerManager
{
    public List<Follower> CurrentFollowers { get; private set; }

    public FollowerManager()
    {
        CurrentFollowers = new List<Follower>();
    }

    public void AddFollower(Follower f)
    {
        if (f == null) { return; }
        CurrentFollowers.Add(f);
    }

}
