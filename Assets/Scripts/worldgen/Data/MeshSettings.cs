
using UnityEngine;

[System.Serializable()]
public class MeshSettings
{
    public const int numSuppoertedLODs = 5;
    public const int numSupportedChunkSizes = 9;
    public const int numSupportedFlatshadedChunkSizes = 3;
    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    [SerializeField]
    private float meshScale = 2.5f;
    
    [SerializeField]
    private bool useFlatShading;

    [SerializeField]
    [Range(0, numSupportedChunkSizes - 1)]
    private int chunkSizeIndex;
    
    [SerializeField]
    [Range(0, numSupportedFlatshadedChunkSizes - 1)]
    private int flatShadedChunkSizeIndex;

    public MeshSettings(float meshScale, bool useFlatShading, int chunkSizeIndex, int flatShadedChunkSizeIndex)
    {
        this.meshScale = meshScale;
        this.useFlatShading = useFlatShading;
        this.chunkSizeIndex = chunkSizeIndex;
        this.flatShadedChunkSizeIndex = flatShadedChunkSizeIndex;
    }
    
    // number of verticies per line of mesh rendered at LOD = 0. Includes the two extra verticies that are excluded
    // from final mesh , but used for calculating normals.    
    public int NumVertsPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 1;
        }
    }

    public float MeshWorldSize
    {
        get
        {
            return (NumVertsPerLine - 3) * meshScale;
        }
    }

    public float MeshScale
    {
        get { return meshScale;}
    }
    
    public bool UseFlatShading
    {
        get { return useFlatShading; }
    }

    public int ChunkSizeIndex
    {
        get { return chunkSizeIndex; }
    }

    public int FlatShadedChunkSizeIndex 
    {
        get { return flatShadedChunkSizeIndex; }
    }

}
