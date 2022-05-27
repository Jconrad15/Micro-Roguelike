using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectFromStringArray
{
    public static string RandomSelect(string[] inputStringArray = null, List<string> inputStringList = null)
    {
        string selectedString;
        if (inputStringArray != null)
        {
            int selectedValue = Random.Range(0, inputStringArray.Length);
            selectedString = inputStringArray[selectedValue].Replace("\r", "");
        }
        else if (inputStringList != null)
        {
            int selectedValue = Random.Range(0, inputStringList.Count);
            selectedString = inputStringList[selectedValue].Replace("\r", "");
        }
        else
        {
            Debug.LogError("Something Bad Happened");
            int selectedValue = Random.Range(0, inputStringArray.Length);
            selectedString = inputStringArray[selectedValue].Replace("\r", "");
        }
        return selectedString;
    }

    public static string WeightedSelect(List<int> weightsList,
                                        string[] inputStringArray = null,
                                        List<string> inputStringList = null)
    {
        string selectedString;
        if (inputStringArray != null)
        {
            int selectedValue;
            // Get the total sum of all the weights.
            int weightSum = 0;
            for (int i = 0; i < weightsList.Count; i++)
            {
                weightSum += weightsList[i];
            }

            int index = 0;
            int lastIndex = weightsList.Count;
            while (index < lastIndex)
            {
                // Do a probability check with a likelihood of weights[index] / weightSum.
                if (Random.Range(0, weightSum) < weightsList[index])
                {
                    selectedValue = index;
                    selectedString = inputStringArray[selectedValue].Replace("\r", "");
                    return selectedString;
                }

                // Remove the last item from the sum of total untested weights and try again.
                weightSum -= weightsList[index++];
            }
            // No other item was selected, so return very last index.
            selectedValue = lastIndex;
            selectedString = inputStringArray[selectedValue].Replace("\r", "");
            return selectedString;

        }
        else if (inputStringList != null)
        {
            int selectedValue;
            // Get the total sum of all the weights.
            int weightSum = 0;
            for (int i = 0; i < weightsList.Count; i++)
            {
                weightSum += weightsList[i];
            }

            int index = 0;
            int lastIndex = weightsList.Count;
            while (index < lastIndex)
            {
                // Do a probability check with a likelihood of weights[index] / weightSum.
                if (Random.Range(0, weightSum) < weightsList[index])
                {
                    selectedValue = index;
                    selectedString = inputStringList[selectedValue].Replace("\r", "");
                    return selectedString;
                }

                // Remove the last item from the sum of total untested weights and try again.
                weightSum -= weightsList[index++];
            }
            // No other item was selected, so return very last index.
            selectedValue = lastIndex;
            selectedString = inputStringList[selectedValue].Replace("\r", "");
            return selectedString;
        }
        else
        {
            Debug.LogError("Something Bad Happened");
            selectedString = "Something Bad Happened";
            return selectedString;
        }
    }




}