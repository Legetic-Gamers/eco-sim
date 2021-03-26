using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationPreview : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public SimulationSettings simulationSettings;

    public Material terrainMaterial;

    public bool autoUpdate;


    public void DrawMapInEditor()
    {
        //DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLevelOfDetail));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
