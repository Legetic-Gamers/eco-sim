using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationPreview : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public SimulationSettings simulationSettings;

    public Material terrainMaterial;

    public bool autoUpdate;
    void Start()
    {
        DisplaySimulationPreview();
        simulationSettings.OnValuesChanged += OnValuesUpdated;
    }

    public void DisplaySimulationPreview(){
        HeightMap heightMap = GenHeightMap();
        DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, simulationSettings.MeshSettings, 0));
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
    }
    private HeightMap GenHeightMap()
    {
        return HeightMapGenerator.GenerateHeightMap(simulationSettings.MeshSettings.NumVertsPerLine, simulationSettings.MeshSettings.NumVertsPerLine, simulationSettings.HeightMapSettings, Vector2.zero);
    }

    private void OnValuesUpdated(){
        DisplaySimulationPreview();
        meshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
    }
}
