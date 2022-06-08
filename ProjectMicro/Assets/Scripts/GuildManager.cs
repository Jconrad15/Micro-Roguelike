using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager
{
    public Guild[] guilds;

    public GuildManager(int seed)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        int guildCount = Random.Range(3, 5);
        guilds = new Guild[guildCount];

        for (int i = 0; i < guildCount; i++)
        {
            guilds[i] = new Guild(i);
        }

        Random.state = oldState;
    }

    public Guild GetRandomGuild()
    {
        return guilds[Random.Range(0, guilds.Length)];
    }
}
