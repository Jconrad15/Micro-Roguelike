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
    public SerializableAreaData currentLocationData;
    public SerializableAreaData worldData;
    public SerializableAreaData[] allLocationData;
    public MapType currentMapType;
    public int seed;
    public int playerWorldX;
    public int playerWorldY;
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
        AreaData currentLocationData =
            AreaDataManager.Instance.CurrentLocationData;
        AreaData worldData =
            AreaDataManager.Instance.GetWorldData();
        AreaData[] allLocationData =
            AreaDataManager.Instance.GetAllLocationData();

        SerializableAreaData[] serializedAllLocationData =
            new SerializableAreaData[allLocationData.Length];
        for (int i = 0; i < allLocationData.Length; i++)
        {
            serializedAllLocationData[i] =
                SerializeAreaData(allLocationData[i]);
        }

        MapType currentMapType = AreaDataManager.Instance.CurrentMapType;
        int playerWorldX, playerWorldY;
        if (currentMapType == MapType.World)
        {
            // Get player to update player world pos 
            Player player = FindObjectOfType<PlayerController>().GetPlayer();
            playerWorldX = player.X;
            playerWorldY = player.Y;
        }
        else
        {
            (playerWorldX, playerWorldY) =
                AreaDataManager.Instance.GetPlayerWorldPosition();
        }

        // Serialize data to get save data
        SaveData data = new SaveData
        {
            currentLocationData = SerializeAreaData(currentLocationData),
            worldData = SerializeAreaData(worldData),
            allLocationData = serializedAllLocationData,
            currentMapType = currentMapType,
            seed = GameInitializer.Instance.Seed,
            playerWorldX = playerWorldX,
            playerWorldY = playerWorldY,
        };

        return data;
    }

    private static SerializableAreaData SerializeAreaData(AreaData areaData)
    {
        if (areaData == null) { return null; }
        if (areaData.MapData == null) { return null; }

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
                    guild = mapData[i].entity.CurrentGuild,
                    favor = mapData[i].entity.Favor,
                    becomeFollowerThreshold = mapData[i].entity.BecomeFollowerThreshold,
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

        SerializableAreaData serializedAreaData = new SerializableAreaData
        {
            isWorld = areaData.IsWorld,
            mapData = serializedTileData,
            width = areaData.Width,
            height = areaData.Height,
        };
        return serializedAreaData;
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
            savedCurrentLocationData = dataToSave.currentLocationData,
            savedWorldData = dataToSave.worldData,
            SavedAllLocationData = dataToSave.allLocationData,
            savedCurrentMapType = dataToSave.currentMapType,
            savedSeed = dataToSave.seed,
            savedPlayerWorldX = dataToSave.playerWorldX,
            savedPlayerWorldY = dataToSave.playerWorldY,
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
                currentLocationData = ssd.savedCurrentLocationData,
                worldData = ssd.savedWorldData,
                allLocationData = ssd.SavedAllLocationData,
                currentMapType = ssd.savedCurrentMapType,
                seed = ssd.savedSeed,
                playerWorldX = ssd.savedPlayerWorldX,
                playerWorldY = ssd.savedPlayerWorldY,
            };

            // Convert back to mapData tile arrays
            AreaData currentAreaData =
                ReconstructAreaData(loadedData.currentLocationData);
            // If there is no current area data,
            // need to switch it to a blank area data object
            if (currentAreaData == null)
            {
                currentAreaData = new AreaData();
            }

            AreaData worldData =
                ReconstructAreaData(loadedData.worldData);
            if (worldData == null)
            {
                Debug.LogError("Why are we loading null world data");
                worldData = new AreaData();
            }

            AreaData[] allLocationData =
                new AreaData[loadedData.allLocationData.Length];
            for (int i = 0; i < allLocationData.Length; i++)
            {
                allLocationData[i] =
                    ReconstructAreaData(loadedData.allLocationData[i]);
            }

            // Create loaded area data container for all loaded data
            LoadedAreaData loadedAreaData =
                new LoadedAreaData
                (
                    currentAreaData,
                    worldData,
                    allLocationData,
                    loadedData.currentMapType,
                    loadedData.seed,
                    loadedData.playerWorldX,
                    loadedData.playerWorldY
                );

            cbOnDataLoadedFromFile?.Invoke(loadedAreaData);
        }
        else
        {
            Debug.LogError("There is no save data!");
        }
    }

    private static AreaData ReconstructAreaData(
        SerializableAreaData serializableAreaData)
    {
        if (serializableAreaData == null) { return null; }
        if (serializableAreaData.mapData == null) { return null; }

        SerializableTile[] serializedMapData = serializableAreaData.mapData;

        Tile[] loadedMapData = new Tile[serializedMapData.Length];
        List<Entity> entities = new List<Entity>();
        List<Feature> features = new List<Feature>();

        for (int i = 0; i < loadedMapData.Length; i++)
        {
            Entity e = null;
            if (serializedMapData[i].entity != null)
            {
                SerializableEntity serializableEntity =
                    serializedMapData[i].entity;

                if (serializableEntity.type == EntityType.Player)
                {
                    // Recreate entity
                    e = new Player(serializableEntity.type,
                        serializableEntity.inventoryItems,
                        serializableEntity.money,
                        serializableEntity.visibility,
                        serializableEntity.entityName,
                        serializableEntity.characterName,
                        serializableEntity.guild);
                }
                else
                {
                    e = ReconstructAIEntity(serializableEntity);
                }
            }
            // Recreate feature
            Feature f = null;
            if (serializedMapData[i].feature != null)
            {
                SerialziableFeature serialziableFeature =
                    serializedMapData[i].feature;

                f = new Feature(serialziableFeature.type,
                    serialziableFeature.visibility);
            }

            loadedMapData[i] =
                new Tile
                (
                    serializedMapData[i].x,
                    serializedMapData[i].y,
                    serializedMapData[i].type,
                    e,
                    f,
                    serializedMapData[i].item,
                    serializedMapData[i].isWalkable,
                    serializedMapData[i].visibility
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

        AreaData reconstructedAreaData = new AreaData(
            serializableAreaData.isWorld,
            loadedMapData,
            serializableAreaData.width,
            serializableAreaData.height,
            entities,
            features);

        return reconstructedAreaData;
    }

    private static Entity ReconstructAIEntity(
        SerializableEntity serializableEntity)
    {
        Entity e;

        // TODO: Need a better way to convert loaded entity
        // to specific type of ai entity
        if (serializableEntity.entityName == "dog")
        {
            e = new Dog(
                serializableEntity.type,
                serializableEntity.inventoryItems,
                serializableEntity.money,
                serializableEntity.visibility,
                serializableEntity.entityName,
                serializableEntity.characterName);
        }
        else if (serializableEntity.entityName == "merchant")
        {
            e = new Merchant(
                serializableEntity.type,
                serializableEntity.inventoryItems,
                serializableEntity.money,
                serializableEntity.visibility,
                serializableEntity.entityName,
                serializableEntity.characterName,
                serializableEntity.guild,
                serializableEntity.becomeFollowerThreshold);
        }
        else
        {
            Debug.Log("Missing entity name of: " +
                serializableEntity.entityName);
            return null;
        }

        return e;
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
    public SerializableAreaData savedCurrentLocationData;
    public SerializableAreaData savedWorldData;
    public SerializableAreaData[] SavedAllLocationData;
    public MapType savedCurrentMapType;
    public int savedSeed;
    public int savedPlayerWorldX;
    public int savedPlayerWorldY;
}