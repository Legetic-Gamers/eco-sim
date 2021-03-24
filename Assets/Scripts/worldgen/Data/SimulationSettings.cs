using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class SimulationSettings : MonoBehaviour
{
    private TerrainGenerator terrainGenerator;
    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public WaterSettings waterSettings;
    public TextureSettings textureSettings;
    public ObjectPlacementSettings objectPlacementSettings;

    public void StartSimulation()
    {
        terrainGenerator = FindObjectOfType<TerrainGenerator>();
        terrainGenerator.StartSimulation(meshSettings, heightMapSettings, textureSettings, waterSettings, objectPlacementSettings);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
