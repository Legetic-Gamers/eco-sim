using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterTab : SettingsManager
{
    public Toggle stylizedWater;
    public Slider waterLevel;
    public Slider waterSourceDelta;

    protected override void Start()
    {
        base.Start();
        stylizedWater.isOn = simulationSettings.WaterSettings.StylizedWater;
        waterLevel.value = simulationSettings.WaterSettings.WaterLevel;
        waterSourceDelta.value = simulationSettings.WaterSettings.WaterVertexDiff;
    }

    public void SetSettings()
    {
        simulationSettings.WaterSettings = new WaterSettings(
            simulationSettings.WaterSettings.GenerateWater,
            stylizedWater.isOn,
            waterLevel.value,
            waterSourceDelta.value,
            simulationSettings.WaterSettings.Size,
            simulationSettings.WaterSettings.GridSize,
            simulationSettings.WaterSettings.Material,
            simulationSettings.WaterSettings.StylizedMaterial,
            simulationSettings.WaterSettings.WaterObjectPrefab
        );
    }
}
