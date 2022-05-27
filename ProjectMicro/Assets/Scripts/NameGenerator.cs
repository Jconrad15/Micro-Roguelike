using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NameGenerator
{
    private static string[] firstNames;
    private static string[] lastNames;

    public static void LoadData()
    {
        // Load text from file
        TextAsset firstNameTA =
            (TextAsset)Resources.Load("TextAssets/firstNameList");
        TextAsset lastNameListTA =
            (TextAsset)Resources.Load("TextAssets/lastNameList");

        firstNames = TextAssetParser.Parse(firstNameTA);
        lastNames = TextAssetParser.Parse(lastNameListTA);
    }

    public static string GenerateName()
    {
        if (firstNames == null || lastNames == null)
        {
            LoadData();
        }

        string selectedFirstName =
            SelectFromStringArray.RandomSelect(firstNames);

        string selectedLastName =
            SelectFromStringArray.RandomSelect(lastNames);

        return selectedFirstName + " " + selectedLastName;
    }

}
