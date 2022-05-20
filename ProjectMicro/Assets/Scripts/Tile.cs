using System;

[Serializable]
public enum TileType { OpenArea, Wall };
public enum VisibilityLevel { NotVisible, PreviouslySeen, Visible };
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
