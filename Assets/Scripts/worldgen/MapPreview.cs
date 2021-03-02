using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode
    {
        NoiseMap, Mesh, Falloff
    };
    public DrawMode drawMode;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public WaterSettings waterSettings;

    public Material terrainMaterial;

    [Range(0, MeshSettings.numSuppoertedLODs - 1)]
    public int editorPreviewLevelOfDetail;
    public bool autoUpdate;



    public void DrawMapInEditor()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
        HeightMap heightMap = genHeightMap();
        if (drawMode == DrawMode.NoiseMap)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLevelOfDetail));

        }
        else if (drawMode == DrawMode.Falloff)
        {
            DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.numVertsPerLine), 0, 1)));
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }

    private HeightMap genHeightMap()
    {
        return HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, Vector2.zero);
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    private void OnTextureValuesUpdated()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
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


        if (waterSettings.generateWater)
            meshFilter.gameObject.AddComponent<WaterChunk>().Setup(Vector2.zero, waterSettings, heightMapSettings, meshRenderer.bounds.size, meshFilter.gameObject.transform, meshFilter.sharedMesh.vertices);
    }

    private void OnDestroy()
    {

    }

    private void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureSettings != null)
        {
            textureSettings.OnValuesUpdated -= OnTextureValuesUpdated;
            textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
        }
        if (waterSettings != null)
        {
            waterSettings.OnValuesUpdated -= OnWaterUpdated;
            waterSettings.OnValuesUpdated += OnWaterUpdated;
        }
    }
}
