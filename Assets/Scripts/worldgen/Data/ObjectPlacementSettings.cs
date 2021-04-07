using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class ObjectPlacementSettings
{
    [SerializeField]
    private List<ObjectType> objectTypes;

    public event Action<int> OnTypeChanged;
    public event Action<string> OnTypeDeleted;
    public event Action<int> OnTypeAdded;

    public IList<ObjectType> ObjectTypes
    {
        get { return objectTypes.AsReadOnly(); }
    }

    public void UpdateIndex(int index, ObjectType objectType)
    {
        //Debug.Log("Updating index: " + index);
        objectTypes[index] = objectType;
        OnTypeChanged.Invoke(index);
    }

    public void AddType(ObjectType objectType)
    {
        // Debug.Log("Adding type");

        objectTypes.Add(objectType);
        OnTypeAdded.Invoke(objectTypes.Count - 1);
    }

    public void RemoveTypeIndex(int index)
    {
        //Debug.Log("Removing index: " + index);
        string name = objectTypes[index].Name;
        objectTypes.RemoveAt(index);
        OnTypeDeleted.Invoke(name);
    }

    public ObjectType GetObjectType(int index)
    {
        return objectTypes[index];
    }
}


[System.Serializable()]
public struct ObjectType
{
    [SerializeField]
    private string name;
    [SerializeField]
    private GameObjectSettings[] gameObjectSettings;
    [SerializeField]
    private float minimumDistance;
    [SerializeField]
    private int newPointCount;
    [SerializeField]
    private float scale;
    [SerializeField]
    private float _yOffset;

    [SerializeField]
    [Range(0, 1)]
    private float minHeight;
    [SerializeField]
    [Range(0, 1)]
    private float maxHeight;

    public ObjectType(string name, GameObjectSettings[] gameObjectSettings, float minimumDistance, int newPointCount, float scale, float yOffset, float minHeight, float maxHeight)
    {
        this.name = name;
        this.gameObjectSettings = gameObjectSettings;
        this.minimumDistance = minimumDistance;
        this.newPointCount = newPointCount;
        this.scale = scale;
        this._yOffset = yOffset;
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
    }

    public string Name
    {
        get { return name; }
    }

    public IList<GameObjectSettings> GameObjectSettings
    {
        get { return Array.AsReadOnly(gameObjectSettings); }
    }

    public float MinimumDistance
    {
        get { return minimumDistance; }
    }

    public int NewPointCount
    {
        get { return newPointCount; }
    }

    public float Scale
    {
        get { return scale; }
    }

    public float yOffset
    {
        get { return _yOffset; }
    }

    public float MinHeight
    {
        get { return minHeight; }
    }

    public float MaxHeight
    {
        get { return maxHeight; }
    }
}

[System.Serializable]
public struct GameObjectSettings
{
    [SerializeField]
    private int gameObjectIndex;
    [SerializeField]
    [Range(0, 1)]
    private float probability;

    public GameObjectSettings(int gameObjectIndex, float probability)
    {
        this.gameObjectIndex = gameObjectIndex;
        this.probability = probability;
    }

    public int GameObjectIndex
    {
        get { return gameObjectIndex; }
    }

    public GameObject GameObject
    {
        get
        {
            return SimulationSettings.instance.preview ? SimulationSettings.instance.availableGameObjects[gameObjectIndex].PreviewObject : SimulationSettings.instance.availableGameObjects[gameObjectIndex].SimulationObject;
        }
    }

    public float Probability
    {
        get { return probability; }
    }
}