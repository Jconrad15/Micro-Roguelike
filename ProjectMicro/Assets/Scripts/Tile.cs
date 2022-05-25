using System;

[Serializable]
public enum TileType { OpenArea, Wall, Water, Grass };
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

    public TileType type;
    public Entity entity;
    public Feature feature;
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
        this.type = type;

        // Start with some null data
        entity = null;
        feature = null;
        neighbors = null;
        item = null;

        Visibility = VisibilityLevel.NotVisible;
        if (type == TileType.Wall)
        {
            isWalkable = false;
        }
    }

    // Constructor for loading tile data from save file
    public Tile(int x, int y, TileType type, Entity entity,
        Feature feature, Item item, bool isWalkable, VisibilityLevel visibility)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.entity = entity;
        this.feature = feature;
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
