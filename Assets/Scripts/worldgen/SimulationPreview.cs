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
        simulationSettings.OnHeightMapChanged += OnTerrainChanged;
        simulationSettings.OnMeshChanged += OnTerrainChanged;
        simulationSettings.OnWaterChanged += OnWaterUpdated;
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

    private void OnTerrainChanged(){
        if(autoUpdate){
            DisplaySimulationPreview();
            meshFilter.gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;
        }
    }

    private void OnWaterUpdated()
    {
        if (meshFilter.gameObject.GetComponent<WaterChunk>() != null)
        {
            var waterChunks = meshFilter.gameObject.GetComponents<WaterChunk>();
            foreach (var waterChunk in waterChunks)
            {
                DestroyImmediate(waterChunk.waterObject);
                DestroyImmediate(waterChunk);
            }
        }
        if (simulationSettings.WaterSettings.GenerateWater)
            meshFilter.gameObject.AddComponent<WaterChunk>().Setup(Vector2.zero, simulationSettings.WaterSettings, simulationSettings.HeightMapSettings, meshRenderer.bounds.size, meshFilter.gameObject.transform, meshFilter.sharedMesh.vertices);
    }
}
