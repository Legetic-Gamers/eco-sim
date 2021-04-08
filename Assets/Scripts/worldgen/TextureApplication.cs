using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class TextureApplication : MonoBehaviour
{
    public void UpdateTextureSettings(Color[] baseColours, float[] newBaseHeights, float minHeight, float maxHeight)
    {
        SimulationSettings.instance.TextureSettings = new TextureSettings(
            baseColours,
            newBaseHeights,
            minHeight,
            maxHeight
        );
    }


    public void ApplyToMaterial(Material material)
    {
        // Debug.Log("Texture Settings: " + settings.TextureSettings);
        // Debug.Log("Base colors: " + settings.TextureSettings.BaseColours.Count);
        // Debug.Log("Start Height: " + settings.TextureSettings.BaseStartHeights.Count);

        Color[] colours = SimulationSettings.instance.TextureSettings.BaseColours.ToArray();
        float[] startHeight = SimulationSettings.instance.TextureSettings.BaseStartHeights.ToArray();

        material.SetColor("waterColor", colours[0]);
        material.SetColor("sandColor", colours[1]);
        material.SetColor("grassLowColor", colours[2]);
        material.SetColor("grassHighColor", colours[3]);
        material.SetFloat("waterHeight", startHeight[0]);
        material.SetFloat("sandHeight", startHeight[1]);
        material.SetFloat("grassLowHeight", startHeight[2]);
        material.SetFloat("grassHighHeight", startHeight[3]);
        SetHeightsBasedFromSettings(material);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        SimulationSettings.instance.TextureSettings = new TextureSettings(
            SimulationSettings.instance.TextureSettings.BaseColours.ToArray(),
            SimulationSettings.instance.TextureSettings.BaseStartHeights.ToArray(),
            minHeight,
            maxHeight
        );
        SetHeightsBasedFromSettings(material);
    }

    private void SetHeightsBasedFromSettings(Material material)
    {
        material.SetFloat("minHeight", SimulationSettings.instance.TextureSettings.SavedMinHeight);
        material.SetFloat("maxHeight", SimulationSettings.instance.TextureSettings.SavedMaxHeight);
    }
}