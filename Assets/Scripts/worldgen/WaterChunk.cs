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


    public void Setup(Vector2 position, WaterSettings waterSettings, HeightMapSettings heightMapSettings, Vector3 scale, Transform parent, Vector3[] worldVerticies)
    {
        this.waterSettings = waterSettings;
        this.heightMapSettings = heightMapSettings;
        this.worldVerticies = worldVerticies;

        if (waterSettings.stylizedWater)
        {
            waterObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            waterObject.name = "Water Chunk";
            waterObject.transform.parent = parent;

            meshRenderer = waterObject.GetComponent<MeshRenderer>();

            //waterObject.transform.localScale = parent.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds.size;

            realWaterLevel = Mathf.Lerp(heightMapSettings.minHeight, heightMapSettings.maxHeight, waterSettings.waterLevel);

            waterObject.transform.localScale = new Vector3(scale.x, 0, scale.z);
            waterObject.transform.position = new Vector3(position.x, -waterObject.transform.localScale.y / 2, position.y);

            waterObject.transform.position += new Vector3(0, realWaterLevel / 2, 0);
            waterObject.transform.localScale += new Vector3(0, realWaterLevel, 0);

            meshRenderer.material = waterSettings.stylizedMaterial;

            obstacle = waterObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
        }
        else
        {
            waterObject = new GameObject("Water Chunk");
            waterObject.transform.parent = parent;

            waterObject.transform.position = new Vector3(position.x, 0, position.y);
            meshFilter = waterObject.AddComponent<MeshFilter>();
            meshRenderer = waterObject.AddComponent<MeshRenderer>();
            meshRenderer.material = waterSettings.material;


            meshFilter.mesh = GenerateMesh();
            //waterObject.AddComponent<WaterNoise>();
            //waterObject.GetComponent<WaterNoise>().settings = waterSettings;
            realWaterLevel = Mathf.Lerp(heightMapSettings.minHeight, heightMapSettings.maxHeight, waterSettings.waterLevel);
            waterObject.transform.localScale = scale;
            waterObject.transform.position += new Vector3(0, realWaterLevel, 0);
            collider = waterObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(1, 1, 1);
            collider.center -= new Vector3(0, 0.5f, 0);

            obstacle = waterObject.AddComponent<NavMeshObstacle>();
            obstacle.carving = true;
        }


        PlaceWaterSources();

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

        for (int x = 0; x < waterSettings.gridSize + 1; x++)
        {
            for (int y = 0; y < waterSettings.gridSize + 1; y++)
            {
                verticies.Add(new Vector3(-waterSettings.size * 0.5f + waterSettings.size * (x / ((float)waterSettings.gridSize)), 0, -waterSettings.size * 0.5f + waterSettings.size * (y / ((float)waterSettings.gridSize))));
                normals.Add(Vector3.up);
                uvs.Add(new Vector2(x / (float)waterSettings.gridSize, y / (float)waterSettings.gridSize));
            }

        }

        var triangles = new List<int>();
        var vertCount = waterSettings.gridSize + 1;

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
            if (Mathf.Abs(vert.y - realWaterLevel) <= waterSettings.waterVertexDiff)
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
