using System;

[Serializable]
public class Guild
{
    public int Id { get; protected set; }
    public string GuildName { get; protected set; }

    public Guild(int id)
    {
        Id = id;
        GuildName = GuildNameGenerator.GenerateName();
    }

}
