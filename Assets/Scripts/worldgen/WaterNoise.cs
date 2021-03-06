using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterNoise : MonoBehaviour
{
    public WaterSettings settings;
    private float xOffset;
    private float yOffset;
    private MeshFilter mf;

    private void Start()
    {
        mf = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        xOffset += Time.deltaTime * settings.timeScale;
        yOffset += Time.deltaTime * settings.timeScale;
    }

    private void MakeNoise()
    {
        Vector3[] verticies = mf.mesh.vertices;

        for (int i = 0; i < verticies.Length; i++)
        {
            verticies[i].y = CalculateHeight(verticies[i].x, verticies[i].z) * settings.power;
        }
    }

    private float CalculateHeight(float x, float y)
    {
        float xCoord = x * settings.scale + xOffset;
        float yCoord = y * settings.scale + yOffset;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
