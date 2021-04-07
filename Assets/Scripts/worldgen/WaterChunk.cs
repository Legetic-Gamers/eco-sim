using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaterChunk : MonoBehaviour
{

    WaterSettings waterSettings;
    public GameObject waterObject;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    BoxCollider collider;
    NavMeshObstacle obstacle;
    HeightMapSettings heightMapSettings;
    Vector3[] worldVerticies;
    float realWaterLevel;


    public void Setup(Vector2 position, WaterSettings waterSettings, HeightMapSettings heightMapSettings, Vector3 scale, Transform parent, Vector3[] worldVerticies, bool placeWaterSources)
    {
        this.waterSettings = waterSettings;
        this.heightMapSettings = heightMapSettings;
        this.worldVerticies = worldVerticies;

        waterObject = new GameObject("Water Chunk");
        waterObject.transform.parent = parent;

        meshFilter = waterObject.AddComponent<MeshFilter>();
        meshRenderer = waterObject.AddComponent<MeshRenderer>();
        meshRenderer.material = waterSettings.Material;

        meshFilter.mesh = GenerateMesh();
        //waterObject.AddComponent<WaterNoise>();
        //waterObject.GetComponent<WaterNoise>().settings = waterSettings;
        realWaterLevel = Mathf.Lerp(heightMapSettings.MinHeight, heightMapSettings.MaxHeight, waterSettings.WaterLevel);
        waterObject.transform.localScale = new Vector3(scale.x, 1, scale.z);
        waterObject.transform.position = new Vector3(position.x, realWaterLevel, position.y);
        collider = waterObject.AddComponent<BoxCollider>();
        collider.size = new Vector3(1, realWaterLevel, 1);
        collider.center -= new Vector3(0, 0.5f * realWaterLevel, 0);

        obstacle = waterObject.AddComponent<NavMeshObstacle>();
        obstacle.carving = true;


        if (waterSettings.StylizedWater)
        {
            var stylizedObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stylizedObject.name = "Water Stylized";
            stylizedObject.transform.parent = waterObject.transform;
            stylizedObject.transform.localScale = collider.size;

            if (Application.isEditor)
            {
                DestroyImmediate(stylizedObject.GetComponent<BoxCollider>());
                DestroyImmediate(waterObject.GetComponent<MeshRenderer>());
                DestroyImmediate(waterObject.GetComponent<MeshFilter>());
            }
            else
            {
                Destroy(stylizedObject.GetComponent<BoxCollider>());
                Destroy(waterObject.GetComponent<MeshRenderer>());
                Destroy(waterObject.GetComponent<MeshFilter>());
            }
            var stylizedMeshRenderer = stylizedObject.GetComponent<MeshRenderer>();

            stylizedObject.transform.position = new Vector3(position.x, realWaterLevel / 2, position.y);
            stylizedMeshRenderer.material = waterSettings.StylizedMaterial;
        }

        if (placeWaterSources)
        {
            PlaceWaterSources();
        }

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

        for (int x = 0; x < waterSettings.GridSize + 1; x++)
        {
            for (int y = 0; y < waterSettings.GridSize + 1; y++)
            {
                verticies.Add(new Vector3(-waterSettings.Size * 0.5f + waterSettings.Size * (x / ((float)waterSettings.GridSize)), 0, -waterSettings.Size * 0.5f + waterSettings.Size * (y / ((float)waterSettings.GridSize))));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / (float)waterSettings.GridSize, y / (float)waterSettings.GridSize));
            }

        }

        var triangles = new List<int>();
        var vertCount = waterSettings.GridSize + 1;

        for (int i = 0; i < vertCount * vertCount - vertCount; i++)
        {
            if ((i + 1) % vertCount == 0)
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

    private void PlaceWaterSources()
    {
        foreach (var vert in worldVerticies)
        {
            if (Mathf.Abs(vert.y - realWaterLevel) <= waterSettings.WaterVertexDiff)
            {
                PlaceWaterSource(vert);
            }
        }

    }

    private void PlaceWaterSource(Vector3 pos)
    {
        GameObject waterSource = new GameObject("Water Source");
        waterSource.transform.parent = waterObject.transform;
        waterSource.tag = "Water";
        waterSource.layer = LayerMask.NameToLayer("Target");
        BoxCollider box = waterSource.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(0.5f, 0.5f, 0.5f);
        waterSource.transform.position = pos;
    }

}
