using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private void Start()
    {
        SimulationSettings settings = FindObjectOfType<SimulationSettings>();
        settings.StartSimulation();
    }
}
