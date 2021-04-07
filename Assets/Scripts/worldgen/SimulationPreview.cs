using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationPreview : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public SimulationSettings simulationSettings;
    public TextureApplication textureApplication;

    private ObjectPlacement objectPlacement;


    public Material terrainMaterial;

    public bool autoUpdate;
    void Start()
    {
        DisplaySimulationPreview();
        simulationSettings.OnHeightMapChanged += OnTerrainChanged;
        simulationSettings.OnHeightMapChanged += OnTextureValuesUpdated;
        simulationSettings.OnMeshChanged += OnTerrainChanged;
        simulationSettings.OnMeshChanged += OnWaterUpdated;
        simulationSettings.OnMeshChanged += OnTextureValuesUpdated;
        simulationSettings.OnWaterChanged += OnWaterUpdated;
        simulationSettings.OnTextureChanged += OnTextureValuesUpdated;

        OnWaterUpdated();
        OnTextureValuesUpdated();

        objectPlacement = meshFilter.gameObject.AddComponent<ObjectPlacement>();
        objectPlacement.PlaceObjects(new Vector2(meshFilter.gameObject.transform.position.x, meshFilter.gameObject.transform.position.z));
        simulationSettings.ObjectPlacementSettings.OnTypeAdded += OnObjectTypeAdded;
        simulationSettings.ObjectPlacementSettings.OnTypeChanged += OnTypeChanged;
        simulationSettings.ObjectPlacementSettings.OnTypeDeleted += objectPlacement.DestroyGroupObjectWithName;

    }

    private void OnObjectTypeAdded(int index)
    {
        var objectTypes = simulationSettings.ObjectPlacementSettings.ObjectTypes;
        Debug.Log("Index / value: " + index + " " + objectTypes[index].Name);
        objectPlacement.PlaceObjectType(
            objectTypes[index],
            new Vector2(
                meshFilter.gameObject.transform.position.x,
                meshFilter.gameObject.transform.position.z)
        );
    }

    private void OnTypeChanged(int index)
    {
        objectPlacement.UpdateObjectType(
            index,
            new Vector2(
                meshFilter.gameObject.transform.position.x,
                meshFilter.gameObject.transform.position.z)
        );
    }



    public void DisplaySimulationPreview()
    {

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

    private void OnTerrainChanged()
    {
        if (autoUpdate)
        {
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
            meshFilter.gameObject.AddComponent<WaterChunk>().Setup(Vector2.zero, simulationSettings.WaterSettings, simulationSettings.HeightMapSettings, meshRenderer.bounds.size, meshFilter.gameObject.transform, meshFilter.sharedMesh.vertices, false);
    }

    private void OnTextureValuesUpdated()
    {
        textureApplication.ApplyToMaterial(terrainMaterial);
    }
}
