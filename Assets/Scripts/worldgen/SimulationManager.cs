using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public void StartSimulation()
    {
        SimulationSettings settings = FindObjectOfType<SimulationSettings>();
        TerrainGenerator terrainGenerator = FindObjectOfType<TerrainGenerator>();
        TextureApplication textureApplication = FindObjectOfType<TextureApplication>();
        terrainGenerator.StartSimulation(settings.MeshSettings, settings.HeightMapSettings, settings.TextureSettings, settings.WaterSettings, settings.ObjectPlacementSettings, textureApplication, settings.xFixedSize, settings.yFixedSize);
    }
    private void Start()
    {
        StartSimulation();
    }
}
