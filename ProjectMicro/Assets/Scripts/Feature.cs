using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum FeatureType { Door, City, Town };
[Serializable]
public class SerialziableFeature
{
    public FeatureType type;
    public VisibilityLevel visibility;
}

public class Feature
{
    public FeatureType type;
    public Tile T { get; set; }

    private Action<Feature> cbOnVisibilityChanged;

    private VisibilityLevel visibility;
    public VisibilityLevel Visibility
    {
        get { return visibility; }
        set
        {
            visibility = value;
            cbOnVisibilityChanged?.Invoke(this);
        }
    }

    /// <summary>
    /// Constructor for initial generation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="t"></param>
    public Feature(FeatureType type, Tile t)
    {
        this.type = type;
        T = t;
        Visibility = VisibilityLevel.NotVisible;

        // Add self to feature list
        AreaData areaData = AreaData.GetAreaDataForCurrentType();
        areaData.AddFeature(this);
    }

    /// <summary>
    /// Contructor for loading saved data
    /// </summary>
    public Feature(FeatureType featureType, VisibilityLevel visibility)
    {
        type = featureType;
        this.visibility = visibility;
    }

    public void SetTile(Tile tile)
    {
        if (tile == null) { return; }
        T = tile;
    }

    public void RegisterOnVisibilityChanged(Action<Feature> callbackfunc)
    {
        cbOnVisibilityChanged += callbackfunc;
    }

    public void UnregisterOnVisibilityChanged(Action<Feature> callbackfunc)
    {
        cbOnVisibilityChanged -= callbackfunc;
    }
}
