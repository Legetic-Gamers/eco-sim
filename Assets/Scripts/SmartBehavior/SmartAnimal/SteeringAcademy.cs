using System;
using System.Collections.Generic;
using AnimalsV2.States.AnimalsV2.States;
using Unity.MLAgents;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
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

    private AnimalMovementBrain agentBrain;

    
    public float totalScore;
    public Text scoreText;
    StatsRecorder m_Recorder;

    public void Start()
    {
        m_Recorder = Academy.Instance.StatsRecorder;
        agentBrain = agent.GetComponent<AnimalMovementBrain>();
        agentBrain.onEpisodeEnd += HandleEndEpisode;
        agentBrain.onEpisodeBegin += HandleBeginEpisode;
    }

    public void Update()
    {
        //agents = FindObjectsOfType<AnimalBrainAgent>();

        if(scoreText) scoreText.text = $"Score: {totalScore}";

        // Send stats via SideChannel so that they'll appear in TensorBoard.
        // These values get averaged every summary_frequency steps, so we don't
        // need to send every Update() call.
        if ((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }

    private void HandleBeginEpisode(float temp)
    {
        Debug.Log("Begin episode!");
        foreach (EnvironmentObject envObj in environmentObjects)
        {
            CreateObjectInstances(envObj);
        }
        ResetAgent();
    }

    private void HandleEndEpisode(float temp)
    {
        Debug.Log("End episode!");
        foreach (EnvironmentObject envObj in environmentObjects)
        {
            ClearObjectInstances(envObj);
        }
    }

    //Create all objects of a given environment object
    private void CreateObjectInstances(EnvironmentObject environmentObject)
    {
        //We want to create amountPerRound number of units
        for (int i = 0; i < environmentObject.amountPerRound; i++)
        {
            GameObject obj = Instantiate(environmentObject.prefab, transform);
            obj.transform.localPosition = GetRandomPointOnPlane();
            obj.transform.rotation = GetRandomRotation();
            environmentObject.instances.Add(obj);
        }
    }
    
    //Clear all objects of a given environment object
    void ClearObjectInstances(EnvironmentObject environmentObject)
    {
        foreach (GameObject obj in environmentObject.instances)
        {
            Destroy(obj);
        }
        environmentObject.instances.Clear();
    }
    
    private Vector3 GetRandomPointOnPlane()
    {
        //Get the planes localscale (which is 5m per unit)
        float planeRadiusX = Mathf.Abs(transform.localScale.x) * 5/2;
        float planeRadiusY = Mathf.Abs(transform.localScale.y) * 5/2;
        
        //Give some marginal from walls
        planeRadiusX -= 1;
        planeRadiusY -= 1;
        
        //Get random vector on plane
        return new Vector3(Random.Range(-planeRadiusX, planeRadiusX), transform.position.y,
            Random.Range(-planeRadiusY, planeRadiusY));
    }

    private Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f));
    }
    
    private void ResetAgent()
    {
        if (agent.TryGetComponent(out AnimalController animalController) && animalController.animalModel != null)
        {
            AnimalModel animalModel = animalController.animalModel;
            //MAKE SURE YOU ARE USING LOCAL POSITION
            agent.transform.localPosition = GetRandomPointOnPlane();
            agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            Debug.Log("reset!");
        
        
            animalModel.currentEnergy = animalModel.traits.maxEnergy;
            animalModel.currentSpeed = 0;
            animalModel.currentHealth = animalModel.traits.maxHealth;
            animalModel.currentHydration = animalModel.traits.maxHydration;
            animalModel.reproductiveUrge = 0.2f;
            animalModel.age = 0;
            animalController.fsm.absorbingState = false;    
        }
    }
    
    
    private void OnDestroy()
    {
        agentBrain.onEpisodeEnd -= HandleEndEpisode;
        agentBrain.onEpisodeBegin -= HandleBeginEpisode;
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