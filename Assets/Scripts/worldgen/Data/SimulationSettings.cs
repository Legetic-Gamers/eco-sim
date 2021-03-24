using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class SimulationSettings : MonoBehaviour
{
    public TerrainGenerator terrainGenerator;
    public HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    public WaterSettings waterSettings;
    public TextureSettings textureSettings;
    public ObjectPlacementSettings objectPlacementSettings;

    public void StartSimulation()
    {
        terrainGenerator.StartSimulation(meshSettings, heightMapSettings, textureSettings, waterSettings, objectPlacementSettings);
    }

    private void Start()
    {
        StartSimulation();
    }
}
