using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public PrefabSimulationManager prefabSimulationManager;
    public TextureApplication textureApplication;
    public TerrainGenerator terrainGenerator;

    public void StartSimulation()
    {
        SimulationSettings settings = FindObjectOfType<SimulationSettings>();
        terrainGenerator.StartSimulation(settings.MeshSettings, settings.HeightMapSettings, settings.TextureSettings, settings.WaterSettings, settings.ObjectPlacementSettings, textureApplication, settings.xFixedSize, settings.yFixedSize);
    }

    public void StartSimulationFromPrefab(GeneralSettings generalSettings)
    {

        SimulationSettings settings = generalSettings.simulationSettings;

        prefabSimulationManager.StartSimulation(settings.ObjectPlacementSettings, textureApplication, generalSettings.pathToString);
    }

    private void Start()
    {
        GeneralSettings generalSettings = FindObjectOfType<GeneralSettings>();

        if (generalSettings.worldType == GeneralSettings.WorldType.GeneratedWorld)
        {
            StartSimulation();
        }
        else if (generalSettings.worldType == GeneralSettings.WorldType.PrefabWorld)
        {
            StartSimulationFromPrefab(generalSettings);
        }

    }
}
