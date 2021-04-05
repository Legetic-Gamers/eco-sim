using UnityEngine.UI;
using TMPro;

public class TextureTab : SettingsManager
{
    public Slider waterHeight;
    public Slider sandHeight;
    public Slider grassHeight;
    public Slider rockHeight;

    protected override void Start()
    {
        base.Start();
        waterHeight.value = simulationSettings.TextureSettings.baseStartHeights[0];
        sandHeight.value  = simulationSettings.TextureSettings.baseStartHeights[1];
        grassHeight.value = simulationSettings.TextureSettings.baseStartHeights[2];
        rockHeight.value  = simulationSettings.TextureSettings.baseStartHeights[3];
    }

    public void SetSettings()
    {
        simulationSettings.TextureSettings = new TextureSettings();
    }
}
