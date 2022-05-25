using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteDatabase : MonoBehaviour
{
    public Dictionary<TileType, Sprite[]> TileDatabase
    { get; protected set; }
    public Dictionary<string, Sprite[]> EntityDatabase
    { get; protected set; }
    public Dictionary<FeatureType, Sprite[]> FeatureDatabase
    { get; protected set; }

    public void CreateDatabases()
    {
        // Load scriptable objects from file

        // Create tile database
        TileTypeSpriteRef[] tileReferences =
            Resources.LoadAll("ScriptableObjects/Tiles",
            typeof(TileTypeSpriteRef))
            .Cast<TileTypeSpriteRef>().ToArray();

        TileDatabase = new Dictionary<TileType, Sprite[]>();
        // Create the database
        for (int i = 0; i < tileReferences.Length; i++)
        {
            TileDatabase.Add(
                tileReferences[i].tileType,
                tileReferences[i].sprites);
        }

        // Create entity database
        EntityTypeSpriteRef[] entityreferences =
            Resources.LoadAll("ScriptableObjects/Entities",
            typeof(EntityTypeSpriteRef))
            .Cast<EntityTypeSpriteRef>().ToArray();

        EntityDatabase = new Dictionary<string, Sprite[]>();
        // Create the database
        for (int i = 0; i < entityreferences.Length; i++)
        {
            EntityDatabase.Add(
                entityreferences[i].entityName,
                entityreferences[i].sprites);
        }

        // Create entity database
        FeatureTypeSpriteRef[] featureTypeSpriteRef=
            Resources.LoadAll("ScriptableObjects/Features",
            typeof(FeatureTypeSpriteRef))
            .Cast<FeatureTypeSpriteRef>().ToArray();

        FeatureDatabase = new Dictionary<FeatureType, Sprite[]>();
        // Create the database
        for (int i = 0; i < featureTypeSpriteRef.Length; i++)
        {
            FeatureDatabase.Add(
                featureTypeSpriteRef[i].featureType,
                featureTypeSpriteRef[i].sprites);
        }
    }
}
