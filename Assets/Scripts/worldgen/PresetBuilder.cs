using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class PresetBuilder : MonoBehaviour
{
    public string prefabName;

    public SimulationSettings simulationSettings;

    public int colliderLevelOfDetailIndex;
    public LODInfo[] detailLevels;
    public NavMeshSurface navMeshSurface;

    public Material terrainMaterial;

    public TextureApplication textureApplication;

    private List<TerrainChunk> fixedSizeChunks = new List<TerrainChunk>();
    private int loadedChunks;

    private GameObject world;
    private List<GameObject> worlds = new List<GameObject>();
    private Action<GameObject, string> IsDoneCallback;

    public void BuildPreset(Action<GameObject, string> IsDone)
    {
        Debug.Log("Started building prefab!");
        this.IsDoneCallback = IsDone;
        textureApplication.ApplyToMaterial(terrainMaterial);
        textureApplication.UpdateMeshHeights(terrainMaterial, simulationSettings.HeightMapSettings.MinHeight, simulationSettings.HeightMapSettings.MaxHeight);
        world = new GameObject("World");
        worlds.Add(world);

        for (int y = 0; y < simulationSettings.yFixedSize; y++)
        {
            for (int x = 0; x < simulationSettings.xFixedSize; x++)
            {
                TerrainChunk newChunk = new TerrainChunk(
                    new Vector2(x - simulationSettings.xFixedSize / 2, y - simulationSettings.yFixedSize / 2),
                    simulationSettings.HeightMapSettings,
                    simulationSettings.MeshSettings,
                    simulationSettings.WaterSettings,
                    simulationSettings.ObjectPlacementSettings,
                    true,
                    detailLevels,
                    colliderLevelOfDetailIndex,
                    world.transform,
                    null,
                    terrainMaterial,
                    OnChunkLoaded
                );
                fixedSizeChunks.Add(newChunk);
                newChunk.Load();
                newChunk.SetCollisionMesh();
            }
        }
    }

    public void ClearWorlds()
    {
        foreach (var world in worlds)
        {
            DestroyImmediate(world);
        }
    }

    public void OnChunkLoaded()
    {
        loadedChunks++;
        Debug.Log("Chunk built! " + loadedChunks + " out of " + simulationSettings.xFixedSize * simulationSettings.yFixedSize);
        if (loadedChunks >= simulationSettings.xFixedSize * simulationSettings.yFixedSize)
        {
            navMeshSurface.BuildNavMesh();
            IsDoneCallback(world, prefabName);
            //NavMesh.pathfindingIterationsPerFrame = (int)Math.Floor(500 * Time.timeScale);
        }
    }
}
