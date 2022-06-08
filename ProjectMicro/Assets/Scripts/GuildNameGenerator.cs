using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GuildNameGenerator
{
    private static string[] guildNamePart1;
    private static string[] guildNamePart2;

    public static void LoadData()
    {
        // Load text from file
        TextAsset guildNamePart1TA =
            (TextAsset)Resources.Load("TextAssets/guildNamePart1");
        TextAsset guildNamePart2TA =
            (TextAsset)Resources.Load("TextAssets/guildNamePart2");

        guildNamePart1 = TextAssetParser.Parse(guildNamePart1TA);
        guildNamePart2 = TextAssetParser.Parse(guildNamePart2TA);
    }

    public static string GenerateName()
    {
        if (guildNamePart1 == null || guildNamePart2 == null)
        {
            LoadData();
        }

        string selectedpart1 =
            SelectFromStringArray.RandomSelect(guildNamePart1);

        string selectedPart2 =
            SelectFromStringArray.RandomSelect(guildNamePart2);

        return selectedpart1 + " " + selectedPart2;
    }



}
