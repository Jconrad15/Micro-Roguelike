using System;

[Serializable]
public enum TileType { OpenArea };
public enum VisibilityLevel { NotVisible, PreviouslySeen, Visible };
public class Tile
{
    public int x;
    public int y;

    public TileType type;
    public Entity entity;
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
        entity = null;
        neighbors = null;
        Visibility = VisibilityLevel.NotVisible;
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
