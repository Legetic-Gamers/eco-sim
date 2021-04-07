using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class HeightMapSettings
{
    [SerializeField]
    private NoiseSettings noiseSettings;

    [SerializeField]
    private bool useFalloff;

    [SerializeField]
    private float heightMultiplier;
    
    [SerializeField]
    private AnimationCurve heightCurve;

    public HeightMapSettings(NoiseSettings noiseSettings, bool useFalloff, float heightMultiplier, AnimationCurve heightCurve){
        this.noiseSettings = noiseSettings;
        this.useFalloff = useFalloff;
        this.heightMultiplier = heightMultiplier;
        this.heightCurve = heightCurve;
    }

    public float MinHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

    public float HeightMultiplier{
        get {
            return heightMultiplier;
        }
    }

    public NoiseSettings NoiseSettings 
    {
        get {
            return noiseSettings;
        }
    }

    public bool UseFalloff {
        get {
            return useFalloff;
        }
    }

    public AnimationCurve HeightCurve{
        get {
            return heightCurve;
        }
    }

}
