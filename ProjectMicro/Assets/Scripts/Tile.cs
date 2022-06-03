using System;

[Serializable]
public enum TileType { OpenArea, Water, Grass, Forest };
[Serializable]
public enum VisibilityLevel { NotVisible, PreviouslySeen, Visible };
[Serializable]
public class SerializableTile
{
    public int x;
    public int y;

    public TileType type;
    public SerializableEntity entity;
    public SerialziableFeature feature;
    public Item item;
    public bool isWalkable;
    public VisibilityLevel visibility;
}

public class Tile
{
    public int x;
    public int y;

    private TileType type;
    public TileType Type
    {
        get => type;
        set
        {
            type = value;
/*            if (value == TileType.Wall)
            {
                isWalkable = false;
            }*/
        }
    }
    public Entity entity;

    private Feature tileFeature;
    public Feature TileFeature 
    { 
        get => tileFeature;
        set
        {
            tileFeature = value;
            if (value != null)
            {
                if (value.type == FeatureType.Wall)
                {
                    isWalkable = false;
                }
            }
        }
    }
    public Item item;
    public bool isWalkable = true;

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

    // NESW
    public Tile[] neighbors;
    private Action<Tile> cbOnVisibilityChanged;

    // Constructor for creating initial tiles
    public Tile(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.Type = type;

        // Start with some null data
        entity = null;
        TileFeature = null;
        neighbors = null;
        item = null;

        Visibility = VisibilityLevel.NotVisible;
/*        if (type == TileType.Wall)
        {
            isWalkable = false;
        }*/
    }

    // Constructor for loading tile data from save file
    public Tile(int x, int y, TileType type, Entity entity,
        Feature feature, Item item, bool isWalkable, VisibilityLevel visibility)
    {
        this.x = x;
        this.y = y;
        this.Type = type;
        this.entity = entity;
        this.TileFeature = feature;
        this.item = item;
        this.isWalkable = isWalkable;
        Visibility = visibility;
    }

    public void SetNeighbors(Tile[] neighbors)
    {
        this.neighbors = neighbors;
    }

    public void RegisterOnVisibilityChanged(Action<Tile> callbackfunc)
    {
        cbOnVisibilityChanged += callbackfunc;
    }

    public void UnregisterOnVisibilityChanged(Action<Tile> callbackfunc)
    {
        cbOnVisibilityChanged -= callbackfunc;
    }
}
