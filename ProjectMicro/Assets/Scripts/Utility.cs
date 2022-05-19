using System;
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
}
