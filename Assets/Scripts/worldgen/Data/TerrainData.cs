﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class TerrainData : UpdatableData
{
    public bool useFalloff;
    public bool useFlatShading;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float uniformScale = 5f;
}
