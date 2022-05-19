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
}
