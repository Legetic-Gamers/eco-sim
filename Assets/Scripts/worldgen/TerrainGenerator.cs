using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class TerrainGenerator : MonoBehaviour
{


    public enum TerrainMode
    {
        Endless,
        Fixed,
    };


    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    [Header("Terrain Mode")]
    public TerrainMode terrainMode;

    [Header("Endless terrain settings")]
    public int colliderLevelOfDetailIndex;
    public LODInfo[] detailLevels;
    public Transform viewer;

    [Header("Fixed terrain settings")]
    public int fixedSizeX;
    public int fixedSizeY;
    public NavMeshSurface navMeshSurface;
    
    [Header("General")]
    public Material mapMaterial;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public WaterSettings waterSettings;


    private Vector2 viewerPosition;
    private Vector2 viewerPositionOld;

    private float meshWorldSize;
    private int chunksVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    private List<TerrainChunk> fixedSizeChunks = new List<TerrainChunk>();
    private int loadedChunks = 0;



    private void Start()
    {
        textureSettings.ApplyToMaterial(mapMaterial);
        textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        if (terrainMode == TerrainMode.Endless)
        {
            float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
            meshWorldSize = meshSettings.meshWorldSize;
            chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshWorldSize);
            UpdateVisibleChunks();
        }
        else if (terrainMode == TerrainMode.Fixed)
        {
            InitializeFixedChunks();
        }
    }

    private void Update()
    {
        if (terrainMode == TerrainMode.Endless)
        {
            viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

            if (viewerPosition != viewerPositionOld)
            {
                foreach (var terrainChunk in visibleTerrainChunks)
                {
                    terrainChunk.UpdateCollisionMesh();
                }
            }

            if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                UpdateVisibleChunks();
            }
            UpdateVisibleChunks();
        }
    }

    private void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoordinates = new HashSet<Vector2>();
        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoordinates.Add(visibleTerrainChunks[i].coordinate);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        int currentChunkCoordinateX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordinateY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoordinate = new Vector2(currentChunkCoordinateX + xOffset, currentChunkCoordinateY + yOffset);
                if (!alreadyUpdatedChunkCoordinates.Contains(viewedChunkCoordinate))
                {
                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoordinate))
                    {
                        terrainChunkDictionary[viewedChunkCoordinate].UpdateTerrainChunk();

                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoordinate, heightMapSettings, meshSettings, waterSettings, false, detailLevels, colliderLevelOfDetailIndex, transform, viewer, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoordinate, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }

    private void InitializeFixedChunks()
    {
        for (int y = 0; y < fixedSizeY; y++)
        {
            for (int x = 0; x < fixedSizeX; x++)
            {
                TerrainChunk newChunk = new TerrainChunk(new Vector2(x - fixedSizeX / 2, y - fixedSizeY / 2), heightMapSettings, meshSettings, waterSettings, true, detailLevels, colliderLevelOfDetailIndex, transform, viewer, mapMaterial, OnChunkLoaded);
                fixedSizeChunks.Add(newChunk);
                newChunk.Load();
                newChunk.SetCollisionMesh();
            }
        }
    }

    public void OnChunkLoaded()
    {
        loadedChunks++;
        if (loadedChunks >= fixedSizeX * fixedSizeY)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        }
        else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSuppoertedLODs - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}

[System.Serializable]
public struct FixedMapSize
{
    public int x;
    public int y;

    public FixedMapSize(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}