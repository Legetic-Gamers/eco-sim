using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class WaterSettings
{
    [SerializeField]
    private bool generateWater = true;
    
    [SerializeField]
    private bool stylizedWater = false;
    
    [SerializeField]
    [Range(0, 1)]
    private float waterLevel;
    
    [SerializeField]
    [Range(0, 1)]
    private float waterVertexDiff = 0.1f;
    
    [SerializeField]
    private float size = 1;
    
    [SerializeField]
    private int gridSize = 16;
    
    [SerializeField]
    private Material material;
    
    [SerializeField]
    private Material stylizedMaterial;

    public WaterSettings(bool generateWater, bool stylizedWater, float waterLevel, float waterVertexDiff, float size, int gridSize, Material material, Material stylizedMaterial) {
        this.generateWater = generateWater;
        this.stylizedWater = stylizedWater;
        this.waterLevel = waterLevel;
        this.waterVertexDiff = waterVertexDiff;
        this.size = size;
        this.gridSize = gridSize;
        this.material = material;
        this.stylizedMaterial = stylizedMaterial;
    }

#region Getters/Setters
    public bool GenerateWater {
        get { return generateWater; }
    }

    public bool StylizedWater {
        get { return stylizedWater; }
    }
    public float WaterLevel {
        get { return waterLevel; }
    }

    public float WaterVertexDiff {
        get { return waterVertexDiff; }
    }

    public float Size {
        get { return size; }
    }

    public int GridSize {
        get { return gridSize; }
    }

    public Material Material {
        get { return material; }
    }

    public Material StylizedMaterial {
        get { return stylizedMaterial; }
    }
#endregion
}