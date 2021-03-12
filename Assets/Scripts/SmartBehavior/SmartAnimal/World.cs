using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This class is heavily inspired by (mostly copied) from the ML-Toolkit FoodCollectorArea class.
/// https://github.com/Unity-Technologies/ml-agents/blob/main/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorArea.cs
/// Used to randomly spawn things in the ML testing environment and reset.
/// </summary>
public class World : MonoBehaviour
{
    public GameObject food;
    public GameObject rabbit;
    public GameObject wolf;
    public GameObject water;
    public int numFood;
    public int numRabbits;
    public int numWolves;
    public int numWater;

    public float range;

    public float foodRespawnRate;

    public AnimalBrainAgent[] agents;
    
    
    public float totalScore;
    public Text scoreText;
    StatsRecorder m_Recorder;

    public void Awake()
    {
        //ResetWorld();
        
        //Academy.Instance.OnEnvironmentReset += ResetWorld;
        m_Recorder = Academy.Instance.StatsRecorder;
        
        InitWorld();
    }

    public void Update()
    {
        agents = FindObjectsOfType<AnimalBrainAgent>();
        
        scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }

    private void CreateObjects(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 0,
                    Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
        }
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var food in objects)
        {
            Destroy(food);
        }
    }

    public void InitWorld()
    {
        InvokeRepeating("SpawnNewFood", 0, foodRespawnRate);
        CreateObjects(numWater, water);
        CreateObjects(numRabbits, rabbit);
        CreateObjects(numWolves, wolf);
    }

    public void ResetWorld()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("Plant"));
        ClearObjects(GameObject.FindGameObjectsWithTag("Water"));

        //Clear wolves
        WolfController[] wolveControllers = GameObject.FindObjectsOfType<WolfController>();
        List<GameObject> wolves = new List<GameObject>();
        foreach (var w in wolveControllers)
        {
            wolves.Append(w.gameObject);
        }
        ClearObjects(wolves.ToArray());
        
        //Clear agents
        foreach (var agent in agents)
        {
            if (agent != null)
            {
                Destroy(agent.gameObject);
            }
        }
        
        //Create things again.
        CreateObjects(numFood, food);
        CreateObjects(numWater, water);
        CreateObjects(numRabbits, rabbit);
        CreateObjects(numWolves,wolf);

        totalScore = 0;

    }
    
    private void SpawnNewFood()
    {
        CreateObjects(numFood, food);
        
    }

    public void SpawnNewRabbit()
    {
        CreateObjects(1,rabbit);
    }
    
}