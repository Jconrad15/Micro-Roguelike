using System;

[Serializable]
public enum MapType { World, Location };
public static class CurrentMapType
{
    private static Action<MapType> cbOnCurrentMapTypeChange;

    public static MapType Type { get; private set; }

    public static void SetCurrentMapType(MapType type)
    {
        if (Type != type)
        {
            cbOnCurrentMapTypeChange?.Invoke(type);
        }

        Type = type;
    }

    public static void RegisterOnCurrentMapTypeChange(
        Action<MapType> callbackfunc)
    {
        cbOnCurrentMapTypeChange += callbackfunc;
    }

    public static void UnregisterOnCurrentMapTypeChange(
        Action<MapType> callbackfunc)
    {
        cbOnCurrentMapTypeChange -= callbackfunc;
    }
}
