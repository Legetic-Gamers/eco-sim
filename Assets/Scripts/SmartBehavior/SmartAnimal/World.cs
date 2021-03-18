using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2.States.AnimalsV2.States;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using ViewController;
using Random = UnityEngine.Random;

/// <summary>
/// This class is heavily inspired by (mostly copied) from the ML-Toolkit FoodCollectorArea class.
/// https://github.com/Unity-Technologies/ml-agents/blob/main/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorArea.cs
/// Used to randomly spawn things in the ML testing environment and reset.
/// </summary>
public class World : MonoBehaviour
{
    //Objects that live in the environment
    [SerializeField] private GameObject food;
    [SerializeField] private GameObject rabbit;
    [SerializeField] private GameObject wolf;
    [SerializeField] private GameObject water;
    
    //Environment parameters.
    [SerializeField] private int numFood;
    [SerializeField] private int numRabbits;
    [SerializeField] private int numWolves;
    [SerializeField] private int numWater;
    [SerializeField] private float foodRespawnRate;
    
    //Specific to the environment size, used for spawning
    [SerializeField] private float range;

    

    public List<Agent> agents;
    public List<WolfController> wolves;
    public List<PlantController> plants;
    public List<GameObject> waters;


    public float totalScore;
    public Text scoreText;
    StatsRecorder m_Recorder;

    public void Awake()
    {
        //ResetWorld();

        //Academy.Instance.OnEnvironmentReset += ResetWorld;
        // Academy.Instance.EnvironmentParameters.GetWithDefault("numFood", 15.0f);
        // Academy.Instance.EnvironmentParameters.GetWithDefault("numRabbits", 3.0f);
        // Academy.Instance.EnvironmentParameters.GetWithDefault("numWolves", 0.0f);
        // Academy.Instance.EnvironmentParameters.GetWithDefault("numWater", 3.0f);
        // Academy.Instance.EnvironmentParameters.GetWithDefault("foodRespawnRate", 100.0f);

        
        
        
        m_Recorder = Academy.Instance.StatsRecorder;

        InitWorld();
    }

    public void Update()
    {
        //agents = FindObjectsOfType<AnimalBrainAgent>();

        scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }

    private void FixedUpdate()
    {
        ResetOnExtinction();
    }

    private void CreateObjects(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            //Instantiate
            GameObject newObject = Instantiate(type, new Vector3(Random.Range(-range, range), 0,
                    Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));

            //Add agents to lists
            Agent agent = newObject.GetComponent<Agent>();
            if (agent)
            {
                if (TryGetComponent(out AnimalBrainAgent animalBrainAgent))
                {
                    animalBrainAgent.world = this;
                }
                
                agents.Add(agent);
            }

            //Add wolves to list.
            WolfController wolf = newObject.GetComponent<WolfController>();
            if (wolf)
            {
                wolves.Add(wolf);
            }

            //Add Water to list.
            if (newObject.CompareTag("Water"))
            {
                waters.Add(newObject);
            }

            //Add plant to list.
            PlantController plant = newObject.GetComponent<PlantController>();
            if (plant)
            {
                plants.Add(plant);
            }


            //Listen to all animals events
            AnimalController animalController = newObject.GetComponent<AnimalController>();
            if (animalController)
            {
                animalController.onBirth += HandleBirth;
                
            }
        }
    }

    private void OnDestroy()
    {
        foreach (Agent a in agents)
        {
            if (a != null)
            {
                AnimalController animalController = a.GetComponent<AnimalController>();
                if (animalController)
                {
                    
                    animalController.onBirth -= HandleBirth;
                    
                }
            }
        }

        foreach (WolfController w in wolves)
        {
            if (w != null)
            {
                AnimalController animalController = w.GetComponent<AnimalController>();
                if (animalController)
                {
                    animalController.onBirth -= HandleBirth;
                    
                }
            }
        }
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            Destroy(obj);
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
        // ClearObjects(GameObject.FindGameObjectsWithTag("Plant"));
        // ClearObjects(GameObject.FindGameObjectsWithTag("Water"));

        //Clear plants
        foreach (var p in plants)
        {
            //Check if not already destroyed.
            if (p != null)
            {
                Destroy(p.gameObject);
            }
        }

        plants.Clear();

        //Clear Waters
        foreach (var w in waters)
        {
            if (w != null)
            {
                Destroy(w.gameObject);
            }
        }

        waters.Clear();

        //Clear wolves
        foreach (var w in wolves)
        {
            if (w != null)
            {
                Destroy(w.gameObject);
            }
        }

        wolves.Clear();


        //Clear agents
        foreach (var agent in agents)
        {
            if (agent != null)
            {
                Destroy(agent.gameObject);
            }
        }

        agents.Clear();

        //Create things again.
        CreateObjects(numFood, food);
        CreateObjects(numWater, water);
        CreateObjects(numRabbits, rabbit);
        CreateObjects(numWolves, wolf);

        totalScore = 0;
    }

    private void HandleBirth(object sender, AnimalController.OnBirthEventArgs e)
    {
        Agent childAgent = e.child.GetComponent<Agent>();
        if (childAgent != null)
        {
            agents.Add(childAgent);
        }
    }

    private void ResetOnExtinction()
    {
        //Reset if all agents are dead.
        if (agents.All(agent => IsDead(agent) ))
        {
            Debug.Log("Extinction");
            ResetWorld();
        }
    }

    private static bool IsDead(Agent agent)
    {
        //agent is dead if nonexistent
        if (agent == null)
        {
            return true;
        }

        //agent is dead if dead
        AnimalController animalController = agent.GetComponent<AnimalController>();
        if (animalController)
        {
            if (animalController.fsm.CurrentState is Dead || !animalController.animalModel.IsAlive)
            {
                return true;
            }
        }
        else
        {
            //Agent is dead if it has no animalController
            return true;
        }


        //else agent is alive
        return false;
    }

    private void SpawnNewFood()
    {
        CreateObjects(numFood, food);
    }

   
}