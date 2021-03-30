using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class SimulationSettings : MonoBehaviour
{
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
    public event System.Action OnObjectPlacementChanged;

    public int xFixedSize;
    public int yFixedSize;


    public HeightMapSettings HeightMapSettings{
        get{ return heightMapSettings; }
        set {
            heightMapSettings = value;
            OnHeightMapChanged?.Invoke();
        }
    }   

    public MeshSettings MeshSettings
    {
        get { return meshSettings; }
        set { 
            meshSettings = value; 
            OnMeshChanged?.Invoke();
        }
    }

    public WaterSettings WaterSettings
    {
        get { return waterSettings; }
        set { 
            waterSettings = value; 
            OnWaterChanged?.Invoke();            
        }
    }
    
    public TextureSettings TextureSettings
    {
        get { return textureSettings; }
        set { 
            textureSettings = value; 
            OnTextureChanged?.Invoke();        
        }
    }

    public ObjectPlacementSettings ObjectPlacementSettings
    {
        get { return objectPlacementSettings; }
        set { 
            objectPlacementSettings = value;
            OnObjectPlacementChanged?.Invoke(); 
        }
    }
    

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
