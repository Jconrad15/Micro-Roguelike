using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextAssetParser
{
    public static string[] Parse(TextAsset textAsset)
    {
        string[] parsedStrings = textAsset.ToString().Split('\n');

        for (int i = 0; i < parsedStrings.Length; i++)
        {
            parsedStrings[i] = parsedStrings[i].Replace("\r", "");
        }

        return parsedStrings;
    }

    public static List<int> ParseInt(TextAsset textAsset)
    {
        string[] parsedStrings = textAsset.ToString().Split('\n');
        int[] values = new int[parsedStrings.Length];

        for (int i = 0; i < parsedStrings.Length; i++)
        {
            parsedStrings[i] = parsedStrings[i].Replace("\r", "");
            values[i] = int.Parse(parsedStrings[i]);
        }

        List<int> valuesList = new List<int>();
        //Convert to list
        for (int i = 0; i < values.Length; i++)
        {
            valuesList.Add(values[i]);
        }

        return valuesList;
    }

}