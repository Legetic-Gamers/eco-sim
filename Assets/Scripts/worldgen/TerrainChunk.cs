using UnityEngine;
using UnityEngine.AI;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 5f;
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public Vector2 coordinate;

    GameObject meshObject;
    Vector2 sampleCentre;
    Bounds bounds;

    HeightMap heightMap;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;


    // Endless terrain
    LODInfo[] detailLevels;
    LevelOfDetailMesh[] levelOfDetailMeshes;
    int colliderLevelOfDetailIndex;

    // Fixed terrain
    bool fixedTerrain;
    TerrainMesh terrainMesh;

    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDistance;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    WaterChunk waterChunk;
    WaterSettings waterSettings;

    ObjectPlacementSettings objectPlacementSettings;

    public TerrainChunk(Vector2 coordinate, HeightMapSettings heightMapSettings, MeshSettings meshSettings, WaterSettings waterSettings, ObjectPlacementSettings objectPlacementSettings, bool fixedTerrain, LODInfo[] detailLevels, int colliderLevelOfDetailIndex, Transform parent, Transform viewer, Material material, System.Action OnChunkLoaded = null)
    {
        this.coordinate = coordinate;
        this.detailLevels = detailLevels;
        this.colliderLevelOfDetailIndex = colliderLevelOfDetailIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;
        this.fixedTerrain = fixedTerrain;
        this.waterSettings = waterSettings;
        this.objectPlacementSettings = objectPlacementSettings;

        sampleCentre = coordinate * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coordinate * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();

        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        meshObject.layer = LayerMask.NameToLayer("Ground");
        SetVisible(false);

        if (fixedTerrain)
        {
            terrainMesh = new TerrainMesh();
            terrainMesh.updateCallback += UpdateTerrainChunk;
            terrainMesh.updateCallback += SetCollisionMesh;
            if (waterSettings.generateWater) terrainMesh.updateCallback += SetWater;
            if (OnChunkLoaded != null)
                terrainMesh.updateCallback += OnChunkLoaded;
            terrainMesh.updateCallback += PlaceObjects;
            SetVisible(true);
        }
        else
        {
            levelOfDetailMeshes = new LevelOfDetailMesh[detailLevels.Length];

            for (int i = 0; i < detailLevels.Length; i++)
            {
                levelOfDetailMeshes[i] = new LevelOfDetailMesh(detailLevels[i].lod);
                levelOfDetailMeshes[i].updateCallback += UpdateTerrainChunk;
                if (i == colliderLevelOfDetailIndex)
                {
                    levelOfDetailMeshes[i].updateCallback += UpdateCollisionMesh;
                }

            }

            maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        }




    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, heightMapSettings, sampleCentre), OnHeightMapReceived);
    }

    private void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    private Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public void UpdateTerrainChunk()
    {
        if (fixedTerrain)
        {
            if (terrainMesh.hasMesh)
            {
                meshFilter.mesh = terrainMesh.mesh;
            }
            else if (!terrainMesh.hasRequestedMesh)
            {
                terrainMesh.RequestMesh(heightMap, meshSettings);
            }
        }
        else
        {
            if (heightMapReceived)
            {
                float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

                bool wasVisible = IsVisible();
                bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LevelOfDetailMesh lodMesh = levelOfDetailMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(heightMap, meshSettings);
                        }
                    }
                }

                if (wasVisible != visible)
                {
                    SetVisible(visible);
                    if (onVisibilityChanged != null)
                    {
                        onVisibilityChanged(this, visible);
                    }
                }
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasSetCollider)
        {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDstFromViewerToEdge < detailLevels[colliderLevelOfDetailIndex].sqrVisibleDistanceThreshold)
            {
                if (!levelOfDetailMeshes[colliderLevelOfDetailIndex].hasRequestedMesh)
                {
                    levelOfDetailMeshes[colliderLevelOfDetailIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (levelOfDetailMeshes[colliderLevelOfDetailIndex].hasMesh)
                {
                    meshCollider.sharedMesh = levelOfDetailMeshes[colliderLevelOfDetailIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }
    }

    public void SetCollisionMesh()
    {
        if (terrainMesh.hasMesh && !hasSetCollider)
        {
            meshCollider.sharedMesh = terrainMesh.mesh;
            hasSetCollider = true;
        }
    }

    public void SetWater()
    {
        waterChunk = meshObject.AddComponent<WaterChunk>();
        waterChunk.Setup(coordinate * meshSettings.meshWorldSize, waterSettings, heightMapSettings, meshRenderer.bounds.size, meshObject.transform, meshFilter.mesh.vertices);
    }

    public void PlaceObjects()
    {
        if (hasSetCollider)
        {
            ObjectPlacement objectPlacement = meshObject.AddComponent<ObjectPlacement>();
            objectPlacement.PlaceObjects(objectPlacementSettings, meshSettings, heightMapSettings);
        }

    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }
}



class TerrainMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;

    public event System.Action updateCallback;

    protected void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;

        updateCallback();
    }

    public virtual void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;

        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, 0), OnMeshDataReceived);
    }
}

class LevelOfDetailMesh : TerrainMesh
{

    int lod;

    public LevelOfDetailMesh(int lod)
    {
        this.lod = lod;
    }


    public override void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;

        ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }
}