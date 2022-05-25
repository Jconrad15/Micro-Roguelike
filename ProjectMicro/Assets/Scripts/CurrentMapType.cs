using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentMapType
{
    public static MapType Type { get; private set; }

    public static void SetCurrentMapType(MapType type)
    {
        Type = type;
    }

}
