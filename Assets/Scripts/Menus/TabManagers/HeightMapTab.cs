using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HeightMapTab : SettingsManager
{
    public TMP_InputField noiseScale;
    public TMP_InputField octaves;
    public TMP_InputField lacunarity;
    public TMP_InputField seed;
    public TMP_InputField heightMultiplier;
    public Slider persistance;

    protected override void Start()
    {
        base.Start();
        HeightMapSettings heightMapSettings = simulationSettings.HeightMapSettings;
        noiseScale.text = heightMapSettings.NoiseSettings.Scale.ToString();
        octaves.text = heightMapSettings.NoiseSettings.Octaves.ToString();
        lacunarity.text = heightMapSettings.NoiseSettings.Lacunarity.ToString();
        seed.text = heightMapSettings.NoiseSettings.Seed.ToString();
        heightMultiplier.text = simulationSettings.HeightMapSettings.HeightMultiplier.ToString();
        persistance.value = heightMapSettings.NoiseSettings.Persistance;
    }


    public void SetSettings()
    {
        simulationSettings.HeightMapSettings = new HeightMapSettings(
            new NoiseSettings(
                simulationSettings.HeightMapSettings.NoiseSettings.NormalizeMode,
                float.Parse(noiseScale.text),
                int.Parse(octaves.text),
                persistance.value,
                float.Parse(lacunarity.text),
                int.Parse(seed.text),
                simulationSettings.HeightMapSettings.NoiseSettings.Offset
            ),
            simulationSettings.HeightMapSettings.UseFalloff,
            float.Parse(heightMultiplier.text),
            simulationSettings.HeightMapSettings.HeightCurve
        );
    }

}
