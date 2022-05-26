using UnityEngine;

public class RawMapData
{
    public TileType[] rawMap;

    /// <summary>
    /// Constructor for world raw map data
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="seed"></param>
    public RawMapData(
        int width, int height, int seed)
    {
        rawMap = CreateWorldRawMapData(width, height, seed);
    }

    private TileType[] CreateWorldRawMapData(
        int width, int height, int seed)
    {
        TileType[] rawMap = new TileType[width * height];

        SimplexNoise.Seed = seed;
        float scale = 0.03f;

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = WorldData.Instance.GetCoordFromIndex(i);

            float sample = SimplexNoise.CalcPixel2D(x, y, scale) / 255f;
            if (sample <= 0.3f)
            {
                rawMap[i] = TileType.Water;
            }
            else if (sample <= 0.5f)
            {
                rawMap[i] = TileType.Forest;
            }
            else
            {
                rawMap[i] = TileType.OpenArea;
            }
        }

        return rawMap;
    }

    /// <summary>
    /// Constructor for location raw map data.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="seed"></param>
    /// <param name="locationTileType"></param>
    public RawMapData(
        int width, int height,
        int seed, TileType locationTileType,
        int worldX, int worldY)
    {
        rawMap = CreateLocationRawMapData(
            width, height, seed, locationTileType,
            worldX, worldY);
    }

    private TileType[] CreateLocationRawMapData(
        int width, int height,
        int seed, TileType locationTileType,
        int worldX, int worldY)
    {
        TileType[] rawMap = new TileType[width * height];
        SimplexNoise.Seed = seed;

        // Parameters with default values
        float scale = 0.05f;
        float waterLevel = 0.15f;
        float grassLevel = 0.3f;


        if (locationTileType == TileType.Water)
        {

        }
        else if (locationTileType == TileType.Forest)
        {

        }
        else if (locationTileType == TileType.OpenArea)
        {

        }
        else
        {
            Debug.Log("Missing a world tile tile when " +
                "creating location raw map");
        }

        for (int i = 0; i < rawMap.Length; i++)
        {
            (int x, int y) = LocationData.Instance.GetCoordFromIndex(i);
            // Translate coordinate using world position
            x = x - (width / 2) + worldX;
            y = y - (height / 2) + worldY;

            float sample = SimplexNoise.CalcPixel2D(x, y, scale) / 255f;
            if (sample <= waterLevel)
            {
                rawMap[i] = TileType.Water;
            }
            else if (sample <= grassLevel)
            {
                rawMap[i] = TileType.Grass;
            }
            else
            {
                rawMap[i] = TileType.OpenArea;
            }
        }

        return rawMap;
    }
}
