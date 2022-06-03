using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;


/// <summary>
/// Data structure for saved data.
/// </summary>
public struct SaveData
{
    public SerializableTile[] mapData;
    public int width;
    public int height;
    public MapType mapType;
    public int seed;
}

/// <summary>
/// Singleton saving class.
/// </summary>
public class SaveSerial : MonoBehaviour
{
    private Action<LoadedAreaData> cbOnDataLoadedFromFile;
    private Action cbDataSaved;

    // Make singleton
    public static SaveSerial Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ButtonToSaveGame()
    {
        SaveGame();
    }

    public void ButtonToLoadGame()
    {
        LoadGame();
    }

    /// <summary>
    /// Obtain the data that needs to be saved from various game systems.
    /// </summary>
    /// <returns></returns>
    private SaveData GetDataToSave()
    {
        // Get area data
        AreaData areaData = AreaData.GetAreaDataForCurrentType();
        Tile[] mapData = areaData.MapData;

        SerializableTile[] serializedTileData =
            new SerializableTile[mapData.Length];

        // Iterate through each tile
        for (int i = 0; i < mapData.Length; i++)
        {
            // Check if an entity is present
            SerializableEntity e = null;
            if (mapData[i].entity != null)
            {
                e = new SerializableEntity
                {
                    x = mapData[i].entity.X,
                    y = mapData[i].entity.Y,
                    entityName = mapData[i].entity.EntityName,
                    characterName = mapData[i].entity.CharacterName,
                    inventoryItems = mapData[i].entity.InventoryItems,
                    money = mapData[i].entity.Money,
                    type = mapData[i].entity.type,
                    visibility = mapData[i].entity.Visibility,
                };
            }

            // Check if a feature is present
            SerialziableFeature f = null;
            if (mapData[i].TileFeature != null)
            {
                f = new SerialziableFeature
                {
                    type = mapData[i].TileFeature.type,
                    visibility = mapData[i].TileFeature.Visibility,
                };
            }

            // Create the serializable tile data item
            // that will be saved for this tile
            serializedTileData[i] = new SerializableTile
            {
                x = mapData[i].x,
                y = mapData[i].y,
                type = mapData[i].Type,
                entity = e,
                feature = f,
                item = mapData[i].item,
                isWalkable = mapData[i].isWalkable,
                visibility = mapData[i].Visibility,
            };
        }

        SaveData data = new SaveData
        {
            mapData = serializedTileData,
            width = areaData.Width,
            height = areaData.Height,
            mapType = CurrentMapType.Type,
            seed = GameInitializer.Instance.Seed,
        };

        return data;
    }

    /// <summary>
    /// Save a binary file.
    /// </summary>
    private void SaveGame()
    {
        SaveData dataToSave = GetDataToSave();

        BinaryFormatter bf = new BinaryFormatter();

        string fileName = Application.persistentDataPath + 
            "/SaveSlot.dat";

        FileStream file = File.Create(fileName);

        SerializableSaveData ssd = new SerializableSaveData
        {
            savedAreaMapData = dataToSave.mapData,
            savedHeight = dataToSave.height,
            savedWidth = dataToSave.width,
            savedMapType = dataToSave.mapType,
            savedSeed = dataToSave.seed,
        };

        bf.Serialize(file, ssd);
        file.Close();
        cbDataSaved?.Invoke();
    }

    /// <summary>
    /// Load a binary file.
    /// </summary>
    private void LoadGame()
    {
        string fileName = Application.persistentDataPath +
            "/SaveSlot.dat";

        if (File.Exists(fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(fileName, FileMode.Open);

            SerializableSaveData ssd =
                (SerializableSaveData)bf.Deserialize(file);
            file.Close();

            SaveData loadedData = new SaveData
            {
                mapData = ssd.savedAreaMapData,
                width = ssd.savedWidth,
                height = ssd.savedHeight,
                mapType = ssd.savedMapType,
                seed = ssd.savedSeed,
            };

            // Convert back to mapData tile array
            Tile[] loadedMapData = new Tile[loadedData.mapData.Length];
            List<Entity> entities = new List<Entity>();
            List<Feature> features = new List<Feature>();

            for (int i = 0; i < loadedMapData.Length; i++)
            {
                Entity e = null;
                if (loadedData.mapData[i].entity != null)
                {
                    SerializableEntity serializableEntity =
                        loadedData.mapData[i].entity;

                    if (serializableEntity.type == EntityType.Player)
                    {
                        // Recreate entity
                        e = new Player(serializableEntity.type,
                            serializableEntity.inventoryItems,
                            serializableEntity.money,
                            serializableEntity.visibility,
                            serializableEntity.entityName,
                            serializableEntity.characterName);
                    }
                    else
                    {
                        e = new AIEntity(serializableEntity.type,
                            serializableEntity.inventoryItems,
                            serializableEntity.money,
                            serializableEntity.visibility,
                            serializableEntity.entityName,
                            serializableEntity.characterName);
                    }
                }
                // Recreate feature
                Feature f = null;
                if (loadedData.mapData[i].feature != null)
                {
                    SerialziableFeature serialziableFeature =
                        loadedData.mapData[i].feature;

                    f = new Feature(serialziableFeature.type,
                        serialziableFeature.visibility);
                }

                loadedMapData[i] =
                    new Tile
                    (
                        loadedData.mapData[i].x,
                        loadedData.mapData[i].y,
                        loadedData.mapData[i].type,
                        e,
                        f,
                        loadedData.mapData[i].item,
                        loadedData.mapData[i].isWalkable,
                        loadedData.mapData[i].visibility
                    );

                // Now that the tile is created,
                // need to set tile on entity and feature
                if (e != null)
                {
                    e.SetTile(loadedMapData[i]);
                    entities.Add(e);
                }
                if (f != null)
                {
                    f.SetTile(loadedMapData[i]);
                    features.Add(f);
                }
            }

            // Create loaded area data container for all loaded data
            LoadedAreaData loadedAreaData =
                new LoadedAreaData
                (
                    loadedMapData,
                    loadedData.width,
                    loadedData.height,
                    entities,
                    features,
                    loadedData.mapType,
                    loadedData.seed
                );

            cbOnDataLoadedFromFile?.Invoke(loadedAreaData);
        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    public void RegisterOnDataLoaded(Action<LoadedAreaData> callbackFunc)
    {
        cbOnDataLoadedFromFile += callbackFunc;
    }

    public void UnregisterOnDataLoaded(Action<LoadedAreaData> callbackFunc)
    {
        cbOnDataLoadedFromFile -= callbackFunc;
    }

    public void RegisterOnDataSaved(Action callbackFunc)
    {
        cbDataSaved += callbackFunc;
    }

    public void UnregisterOnDataSaved(Action callbackFunc)
    {
        cbDataSaved -= callbackFunc;
    }
}

/// <summary>
/// Serializable data class to be saved.
/// </summary>
[Serializable]
class SerializableSaveData
{
    public SerializableTile[] savedAreaMapData;
    public int savedWidth;
    public int savedHeight;
    public MapType savedMapType;
    public int savedSeed;
}