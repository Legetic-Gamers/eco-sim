using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{

    public Color[] baseColours = new Color[4];
    [Range(0, 1)]
    public float[] baseStartHeights = new float[4];
    // [Range(0, 1)]
    // public float[] baseBlends;

    private float savedMinHeight;
    private float savedMaxHeight;




    public void ApplyToMaterial(Material material)
    {
        material.SetColor("waterColor", baseColours[0]);
        material.SetColor("sandColor", baseColours[1]);
        material.SetColor("grassLowColor", baseColours[2]);
        material.SetColor("grassHighColor", baseColours[3]);
        material.SetFloat("waterHeight", baseStartHeights[0]);
        material.SetFloat("sandHeight", baseStartHeights[1]);
        material.SetFloat("grassLowHeight", baseStartHeights[2]);
        material.SetFloat("grassHighHeight", baseStartHeights[3]);
        // material.SetInt("baseColourCount", baseColours.Length);
        // material.SetColorArray("baseColours", baseColours);
        // material.SetFloatArray("baseStartHeights", baseStartHeights);
        // material.SetFloatArray("baseBlends", baseBlends);
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
