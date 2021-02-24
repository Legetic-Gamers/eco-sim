using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    WaterSettings waterSettings;
    public GameObject waterObject;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;


    public void Setup(Vector2 position, WaterSettings waterSettings, HeightMapSettings heightMapSettings, Vector3 scale,  Transform parent)
    {
        this.waterSettings = waterSettings;

        waterObject = new GameObject("Water Chunk");
        waterObject.transform.position = new Vector3(position.x, 0, position.y);
        waterObject.transform.parent = parent;
        meshFilter = waterObject.AddComponent<MeshFilter>();
        meshRenderer = waterObject.AddComponent<MeshRenderer>();


        meshFilter.mesh = GenerateMesh();
        waterObject.AddComponent<WaterNoise>();
        waterObject.GetComponent<WaterNoise>().settings = waterSettings;
        waterObject.transform.localScale = scale;
        waterObject.transform.position += new Vector3(0, Mathf.Lerp(heightMapSettings.minHeight, heightMapSettings.maxHeight, waterSettings.waterLevel), 0);
    }

    private void OnDestroy()
    {
        Destroy(waterObject);
    }


    private Mesh GenerateMesh()
    {
        Mesh mesh = new Mesh();

        var verticies = new List<Vector3>();
        var normals = new List<Vector3>();
        var uvs = new List<Vector2>();

        for(int x = 0; x < waterSettings.gridSize + 1; x++)
        {
            for(int y = 0; y < waterSettings.gridSize + 1; y++)
            {
                verticies.Add(new Vector3(-waterSettings.size * 0.5f + waterSettings.size * (x / ((float) waterSettings.gridSize)), 0, -waterSettings.size * 0.5f + waterSettings.size * (y / ((float)waterSettings.gridSize))));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / (float)waterSettings.gridSize, y / (float)waterSettings.gridSize));
            }

        }

        var triangles = new List<int>();
        var vertCount = waterSettings.gridSize + 1;

        for(int i = 0; i < vertCount * vertCount - vertCount; i++)
        {
            if((i+1) % vertCount == 0)
            {
                continue;
            }
            triangles.AddRange(new List<int>() { 
                i+1+vertCount, i+vertCount, i,
                i, i+1, i+vertCount+1
            }); 
        }

        mesh.SetVertices(verticies);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        return mesh;
    }

}
