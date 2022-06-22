using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GuildNameGenerator
{
    private static string[] guildNamePart1;
    private static string[] guildNamePart2;
    private static string[] guildNameFull;

    public static void LoadData()
    {
        // Load text from file
        TextAsset guildNamePart1TA =
            (TextAsset)Resources.Load("TextAssets/guildNamePart1");
        TextAsset guildNamePart2TA =
            (TextAsset)Resources.Load("TextAssets/guildNamePart2");
        TextAsset guildNameFullTA =
            (TextAsset)Resources.Load("TextAssets/guildNameFull");

        guildNamePart1 = TextAssetParser.Parse(guildNamePart1TA);
        guildNamePart2 = TextAssetParser.Parse(guildNamePart2TA);
        guildNameFull = TextAssetParser.Parse(guildNameFullTA);
    }

    public static string GenerateName(int seed = 0)
    {
        Random.State oldState = Random.state;
        Random.InitState(seed);

        if (guildNamePart1 == null ||
            guildNamePart2 == null ||
            guildNameFull == null)
        {
            LoadData();
        }

        string name;
        // Choose between full name or parts
        if (Random.value > 0.5f)
        {
            string selectedpart1 =
                SelectFromStringArray.RandomSelect(guildNamePart1);
            string selectedPart2 =
                SelectFromStringArray.RandomSelect(guildNamePart2);
            name = selectedpart1 + " " + selectedPart2;
        }
        else
        {
            name = SelectFromStringArray.RandomSelect(guildNameFull);
        }

        Random.state = oldState;
        return name;
    }

}
