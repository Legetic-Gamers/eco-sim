using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class HeightMapSettings
{
    public NoiseSettings noiseSettings;

    public bool useFalloff;

    public float heightMultiplier;
    public AnimationCurve heightCurve;


    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }
}
