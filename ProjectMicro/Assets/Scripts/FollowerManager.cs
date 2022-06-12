using System.Collections.Generic;

public class FollowerManager
{
    public List<Follower> CurrentFollowers { get; private set; }

    public FollowerManager()
    {
        CurrentFollowers = new List<Follower>();
    }

    public Follower AddFollower(Entity entity)
    {
        if (entity == null) { return null; }

        Follower f = new Follower(entity);
        CurrentFollowers.Add(f);

        return f;
    }

    public void RemoveFollower(Entity entity)
    {

    }

}
