using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    protected SimulationSettings simulationSettings;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        simulationSettings = SimulationSettings.instance;
    }
}
