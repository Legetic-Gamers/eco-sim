using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WaterSettings : UpdatableData
{
    public bool generateWater = true;
    [Range(0, 1)]
    public float waterLevel;
    public float size = 1;
    public int gridSize = 16;
    public float power = 3;
    public float scale = 1;
    public float timeScale = 1;
    public Material material;
}