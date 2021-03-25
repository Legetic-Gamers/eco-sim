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
        HeightMapSettings heightMapSettings = simulationSettings.heightMapSettings;
        noiseScale.text = heightMapSettings.noiseSettings.scale.ToString();
        octaves.text = heightMapSettings.noiseSettings.octaves.ToString();
        lacunarity.text = heightMapSettings.noiseSettings.lacunarity.ToString();
        seed.text = heightMapSettings.noiseSettings.seed.ToString();
        heightMultiplier.text = simulationSettings.heightMapSettings.heightMultiplier.ToString();
        persistance.value = heightMapSettings.noiseSettings.persistance;
    }


    public void SetSettings()
    {
        HeightMapSettings heightMapSettings = simulationSettings.heightMapSettings;
        heightMapSettings.noiseSettings.scale = float.Parse(noiseScale.text);
        heightMapSettings.noiseSettings.octaves = int.Parse(octaves.text);
        heightMapSettings.noiseSettings.lacunarity = float.Parse(lacunarity.text);
        heightMapSettings.noiseSettings.seed = int.Parse(seed.text);
        simulationSettings.heightMapSettings.heightMultiplier = float.Parse(heightMultiplier.text);
        Debug.Log("Persistance Value: " + persistance.value);
        heightMapSettings.noiseSettings.persistance = persistance.value;
    }

}
