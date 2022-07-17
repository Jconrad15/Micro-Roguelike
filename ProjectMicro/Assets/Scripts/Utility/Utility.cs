using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <summary>
    /// Returns a random Enum of given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomEnum<T>()
    {
        Array enumArray = Enum.GetValues(typeof(T));
        T selectedEnum = (T)enumArray.GetValue(
            UnityEngine.Random.Range(0, enumArray.Length));
        return selectedEnum;
    }

    public static float ColorDifference(Color color1, Color color2)
    {
        float dr = Mathf.Abs(color1.r - color2.r);
        float dg = Mathf.Abs(color1.g - color2.g);
        float db = Mathf.Abs(color1.b - color2.b);
        float da = Mathf.Abs(color1.a - color2.a);

        return dr + dg + db + da;
    }

    public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant.IsSubclassOf(potentialBase)
               || potentialDescendant == potentialBase;
    }

    /// <summary>
    /// Shuffle array values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rng"></param>
    /// <param name="array"></param>
    public static void ShuffleArray<T>(T[] array)
    {
        var rng = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }

}
