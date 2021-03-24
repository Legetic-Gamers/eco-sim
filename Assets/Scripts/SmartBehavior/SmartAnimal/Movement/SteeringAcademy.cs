using System;
using System.Collections.Generic;
using AnimalsV2.States.AnimalsV2.States;
using Unity.MLAgents;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// This class is heavily inspired by (mostly copied) from the ML-Toolkit FoodCollectorArea class.
/// https://github.com/Unity-Technologies/ml-agents/blob/main/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorArea.cs
/// Used to randomly spawn things in the ML testing environment and reset.
/// </summary>
public class SteeringAcademy : MonoBehaviour
{
    
    [SerializeField] private GameObject agent;
    [SerializeField] private List<EnvironmentObject> environmentObjects;

    private AnimalController animalController;
    
    private float rangeX;
    private float rangeZ;
    
    private IAgent agentBrain;

    public NavMeshSurface surface;
    
    public float totalScore;
    public Text scoreText;
    StatsRecorder m_Recorder;

    public void Awake()
    {
        //Randomize environment size
        float scaleX = Academy.Instance.EnvironmentParameters.GetWithDefault("envScaleX", 0.8f);
        float scaleZ = Academy.Instance.EnvironmentParameters.GetWithDefault("envScaleZ", 0.8f);
        //Set size of environment
        transform.localScale = new Vector3(scaleX,1f,scaleZ);
        //Set spawn bounds
        rangeX = 5f * scaleX - 0.5f;
        rangeZ = 5f * scaleZ - 0.5f;
        //Build Navmesh
        
        //surface.BuildNavMesh();
        m_Recorder = Academy.Instance.StatsRecorder;
        agentBrain = agent.GetComponent<IAgent>();
        agentBrain.onEpisodeBegin += PopulateEnvironment;
        agentBrain.onEpisodeEnd += ClearEnvironment;
        animalController = agent.GetComponent<AnimalController>();
    }

    public void Update()
    {

        if(scoreText) scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
        
    }

    private void PopulateEnvironment(float temp)
    {
        foreach (EnvironmentObject envObj in environmentObjects)
        {
            CreateObjectInstances(envObj);
        }
        ResetAgent();
    }

    private void ClearEnvironment(float temp)
    {
        foreach (EnvironmentObject envObj in environmentObjects)
        {
            RemoveObjectInstances(envObj);
        }
    }

    //Create all objects of a given environment object
    private void CreateObjectInstances(EnvironmentObject environmentObject)
    {
        //We want to create amountPerRound number of units
        for (int i = 0; i < environmentObject.amountPerRound; i++)
        {
            GameObject obj = Instantiate(environmentObject.prefab, transform, false);
            obj.transform.position = obj.transform.position +
                                     new Vector3(Random.Range(-rangeX, rangeX), 0, Random.Range(-rangeZ, rangeZ));
            environmentObject.instances.Add(obj);
        }
    }
    
    //Clear all objects of a given environment object
    void RemoveObjectInstances(EnvironmentObject environmentObject)
    {
        foreach (GameObject obj in environmentObject.instances)
        {
            Destroy(obj);
        }
        environmentObject.instances.Clear();
    }
    
    
    private void ResetAgent()
    {
        if (agent.TryGetComponent(out AnimalController animalController) && animalController.animalModel != null)
        {
            AnimalModel animalModel = animalController.animalModel;
            
            //MAKE SURE YOU ARE USING LOCAL POSITION
            agent.transform.localPosition = new Vector3(Random.Range(-rangeX, rangeX), 0,
                Random.Range(-rangeZ, rangeZ));
            agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f));

            animalModel.currentEnergy = animalModel.traits.maxEnergy;
            animalModel.currentHealth = animalModel.traits.maxHealth;
            animalModel.currentHydration = animalModel.traits.maxHydration;
            animalModel.reproductiveUrge = 0.2f;
            animalModel.age = 0;
            animalController.fsm.absorbingState = false;
            animalController.agent.ResetPath();
        }
    }
    
    
    private void OnDestroy()
    {
        agentBrain.onEpisodeEnd -= ClearEnvironment;
        agentBrain.onEpisodeBegin -= PopulateEnvironment;    
    }

}

[Serializable]
public class EnvironmentObject
{

    public enum Tags
    {
        Animal, Water, Plant
    }

    [SerializeField] public Tags tag;
    [SerializeField] public GameObject prefab;
    [SerializeField] public List<GameObject> instances;
    [SerializeField] public int amountPerRound;

    public string GetTag()
    {
        return tag.ToString();
    }
    
}
