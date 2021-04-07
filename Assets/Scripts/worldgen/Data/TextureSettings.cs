using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public class TextureSettings
{
    [SerializeField]
    private Color[] baseColours;
    [SerializeField]
    [Range(0, 1)]
    private float[] baseStartHeights;

    private float savedMinHeight;
    private float savedMaxHeight;

    public TextureSettings(Color[] baseColours, float[] baseStartHeights, float savedMinHeight, float savedMaxHeight)
    {
        this.baseColours = baseColours;
        this.baseStartHeights = baseStartHeights;
        this.savedMinHeight = savedMinHeight;
        this.savedMaxHeight = savedMaxHeight;
    }

    public IList<Color> BaseColours
    {
        get { return Array.AsReadOnly(baseColours); }
    }

    public IList<float> BaseStartHeights
    {
        get { return Array.AsReadOnly(baseStartHeights); }
    }


    public float SavedMinHeight
    {
        get
        {
            return savedMinHeight;
        }
    }

    public float SavedMaxHeight
    {
        get
        {
            return savedMaxHeight;
        }
    }
}
