using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class WaterSettings
{
    public bool generateWater = true;
    public bool stylizedWater = false;
    [Range(0, 1)]
    public float waterLevel;
    [Range(0, 1)]
    public float waterVertexDiff = 0.1f;
    public float size = 1;
    public int gridSize = 16;
    public float power = 3;
    public float scale = 1;
    public float timeScale = 1;
    public Material material;
    public Material stylizedMaterial;
}