using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class SimulationSettings : MonoBehaviour
{
    public static SimulationSettings instance;

    private TerrainGenerator terrainGenerator;

    [SerializeField]
    private HeightMapSettings heightMapSettings;

    [SerializeField]
    private MeshSettings meshSettings;

    [SerializeField]
    private WaterSettings waterSettings;

    [SerializeField]
    private TextureSettings textureSettings;

    [SerializeField]
    private ObjectPlacementSettings objectPlacementSettings;


    public event System.Action OnHeightMapChanged;
    public event System.Action OnMeshChanged;
    public event System.Action OnWaterChanged;
    public event System.Action OnTextureChanged;

    public bool preview;

    public GameObjectPair[] availableGameObjects;

    public int xFixedSize;
    public int yFixedSize;


    public HeightMapSettings HeightMapSettings
    {
        get { return heightMapSettings; }
        set
        {
            heightMapSettings = value;
            OnHeightMapChanged?.Invoke();
        }
    }

    public MeshSettings MeshSettings
    {
        get { return meshSettings; }
        set
        {
            meshSettings = value;
            OnMeshChanged?.Invoke();
        }
    }

    public WaterSettings WaterSettings
    {
        get { return waterSettings; }
        set
        {
            waterSettings = value;
            OnWaterChanged?.Invoke();
        }
    }

    public TextureSettings TextureSettings
    {
        get { return textureSettings; }
        set
        {
            textureSettings = value;
            OnTextureChanged?.Invoke();
        }
    }

    public ObjectPlacementSettings ObjectPlacementSettings
    {
        get { return objectPlacementSettings; }
    }


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

    }
}

[System.Serializable]
public struct GameObjectPair
{
    [SerializeField]
    private GameObject simulationObject;
    [SerializeField]
    private GameObject previewObject;

    public GameObjectPair(GameObject simulationObject, GameObject previewObject)
    {
        this.simulationObject = simulationObject;
        this.previewObject = previewObject;
    }

    public GameObject SimulationObject
    {
        get { return simulationObject; }
    }

    public GameObject PreviewObject
    {
        get { return previewObject; }
    }
}
