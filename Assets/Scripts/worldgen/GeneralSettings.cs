using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSettings : MonoBehaviour
{
    public enum WorldType
    {
        GeneratedWorld,
        PrefabWorld
    }

    public SimulationSettings simulationSettings;

    public string pathToString;

    public WorldType worldType;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
