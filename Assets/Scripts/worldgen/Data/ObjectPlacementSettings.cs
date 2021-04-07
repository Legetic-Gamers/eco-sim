using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ObjectPlacementSettings : UpdatableData
{

    public ObjectType[] objectTypes;

}


[System.Serializable]
public struct ObjectType
{
    public string name;
    public GameObjectSettings[] gameObjectSettings;
    public Color previewColour;
    public float minimumDistance;
    public int newPointCount;
    public float scale;
    public float yOffset;
    
    [Range(0, 1)]
    public float minHeight;
    [Range(0, 1)]
    public float maxHeight;
}

[System.Serializable]
public struct GameObjectSettings
{
    public GameObject gameObject;
    [Range(0, 1)]
    public float probability;
}