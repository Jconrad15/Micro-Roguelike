using System;
using System.Collections.Generic;
using UnityEngine;

public enum FeatureType { Door };
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

    public Feature(FeatureType type, Tile t)
    {
        this.type = type;
        T = t;
        Visibility = VisibilityLevel.NotVisible;

        // Add self to feature list
        WorldData.Instance.AddFeature(this);
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
