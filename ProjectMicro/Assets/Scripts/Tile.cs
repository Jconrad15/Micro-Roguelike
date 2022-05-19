using System;

[Serializable]
public enum TileType { OpenArea };
public class Tile
{
    public int x;
    public int y;

    public TileType type;
    public Entity entity;

    // NESW
    public Tile[] neighbors;

    public Tile(int x, int y, TileType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        entity = null;
        neighbors = null;
    }

    public void SetNeighbors(Tile[] neighbors)
    {
        this.neighbors = neighbors;
    }

}
