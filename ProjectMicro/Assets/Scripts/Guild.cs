using System;

[Serializable]
public class Guild
{
    public int Id { get; protected set; }
    public string GuildName { get; protected set; }

    public Guild(int id, int seed = 0)
    {
        Id = id;
        GuildName = GuildNameGenerator.GenerateName(seed);
    }

}
