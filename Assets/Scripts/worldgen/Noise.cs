using System;
using System.Collections;
using UnityEngine;

public static class Noise
{

    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(settings.Seed);
        Vector2[] octaveOffsets = new Vector2[settings.Octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < settings.Octaves; i++)
        {
            float offsetX = prng.Next(-100_000, 100_000) + settings.Offset.x + sampleCentre.x;
            float offsetY = prng.Next(-100_000, 100_000) - settings.Offset.y - sampleCentre.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.Persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < settings.Octaves; i++)
                {

                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.Scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.Scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.Persistance;
                    frequency *= settings.Lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;

                if (settings.NormalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight * 0.9f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.NormalizeMode == NormalizeMode.Local)
        {


            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {

                    //Prefered method for non endless method
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);


                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    [SerializeField]
    private Noise.NormalizeMode normalizeMode;

    [SerializeField]
    private float scale = 50;

    [SerializeField]
    private int octaves = 6;

    [SerializeField]
    [Range(0, 1)]
    private float persistance = .6f;

    [SerializeField]
    private float lacunarity = 2;

    [SerializeField]
    private int seed;

    [SerializeField]
    private Vector2 offset;

    public NoiseSettings(Noise.NormalizeMode normalizeMode, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset)
    {
        this.normalizeMode = normalizeMode;
        this.scale = scale;
        this.octaves = octaves;
        this.persistance = persistance;
        this.lacunarity = lacunarity;
        this.seed = seed;
        this.offset = offset;
    }


    #region Getters    
    public Noise.NormalizeMode NormalizeMode
    {
        get
        {
            return normalizeMode;
        }
    }

    public float Scale
    {
        get
        {
            return scale;
        }
    }

    public int Octaves
    {
        get
        {
            return octaves;
        }
    }

    public float Persistance
    {
        get
        {
            return persistance;
        }
    }

    public float Lacunarity
    {
        get
        {
            return lacunarity;
        }
    }

    public int Seed
    {
        get
        {
            return seed;
        }
    }

    public Vector2 Offset
    {
        get
        {
            return offset;
        }
    }
    #endregion

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}