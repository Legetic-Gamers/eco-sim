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

    public event System.Action OnValuesChanged;

    public int xFixedSize;
    public int yFixedSize;


    public HeightMapSettings HeightMapSettings{
        get{ return heightMapSettings; }
        set {
            heightMapSettings = value;
            OnValuesChanged?.Invoke();
        }
    }   

    public MeshSettings MeshSettings
    {
        get { return meshSettings; }
        set { 
            meshSettings = value; 
            OnValuesChanged?.Invoke();
        }
    }

    public WaterSettings WaterSettings
    {
        get { return waterSettings; }
        set { 
            waterSettings = value; 
            OnValuesChanged?.Invoke();            
        }
    }
    
    public TextureSettings TextureSettings
    {
        get { return textureSettings; }
        set { 
            textureSettings = value; 
            OnValuesChanged?.Invoke();        
        }
    }

    public ObjectPlacementSettings ObjectPlacementSettings
    {
        get { return objectPlacementSettings; }
        set { 
            objectPlacementSettings = value;
            OnValuesChanged?.Invoke(); 
        }
    }
    

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
