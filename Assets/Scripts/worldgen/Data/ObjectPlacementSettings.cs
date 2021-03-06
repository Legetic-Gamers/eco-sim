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
    public GameObject gameObject;
    public Color previewColour;
    public float minimumDistance;
    public int newPointCount;
    [Range(0, 1)]
    public float minHeight;
    [Range(0, 1)]
    public float maxHeight;
}