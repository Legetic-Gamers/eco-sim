using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrefabSimulationManager : MonoBehaviour
{

    public Material terrainMaterial;

    private ObjectPlacementSettings objectPlacementSettings;

    public void StartSimulation(ObjectPlacementSettings objectPlacementSettings, TextureApplication textureApplication, string nameOfWorld)
    {
        string basePath = "Worlds/";

        GameObject world = Instantiate(Resources.Load(basePath + nameOfWorld) as GameObject, Vector3.zero, Quaternion.Euler(Vector3.zero));
        SimulationSettings simulationSettingsFromPrefab = world.GetComponentInChildren<SimulationSettings>();
        NavMeshData navMeshData = Resources.Load(basePath + "NavMesh-" + nameOfWorld) as NavMeshData;
        Debug.Log("NavMeshData: " + navMeshData);
        NavMeshSurface navMeshSurface = world.GetComponentInChildren<NavMeshSurface>();
        navMeshSurface.navMeshData = navMeshData;

        textureApplication.simulationSettings = simulationSettingsFromPrefab;
        textureApplication.ApplyToMaterial(terrainMaterial);
        textureApplication.UpdateMeshHeights(terrainMaterial, simulationSettingsFromPrefab.HeightMapSettings.MinHeight, simulationSettingsFromPrefab.HeightMapSettings.MaxHeight);

        // simulationSettingsFromPrefab;
        ObjectPlacement objectPlacement = gameObject.AddComponent<ObjectPlacement>();
        Debug.Log("Started placing objects!");
        objectPlacement.PlaceObjects(simulationSettingsFromPrefab.MeshSettings, objectPlacementSettings, simulationSettingsFromPrefab.xFixedSize, simulationSettingsFromPrefab.yFixedSize);
        ObjectPooler.Instance.HandleFinishedSpawning();
        Destroy(simulationSettingsFromPrefab);
    }
}
