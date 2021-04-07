using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextureTab : SettingsManager
{
    public Slider waterHeight;
    public Slider sandHeight;
    public Slider grassHeight;
    public Slider rockHeight;

    public TextureApplication textureApplication;

    protected override void Start()
    {
        base.Start();
        float[] startHeight = simulationSettings.TextureSettings.BaseStartHeights.ToArray();
        waterHeight.value = startHeight[0];
        sandHeight.value = startHeight[1];
        grassHeight.value = startHeight[2];
        rockHeight.value = startHeight[3];
    }

    public void SetSettings()
    {
        textureApplication.UpdateTextureSettings(
            simulationSettings.TextureSettings.BaseColours.ToArray(),
            new float[] { waterHeight.value, sandHeight.value, grassHeight.value, rockHeight.value },
            simulationSettings.HeightMapSettings.MinHeight,
            simulationSettings.HeightMapSettings.MaxHeight
        );
    }
}
