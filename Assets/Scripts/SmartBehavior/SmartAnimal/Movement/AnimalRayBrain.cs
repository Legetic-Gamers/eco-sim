using System;
using System.Linq;
using System.Numerics;
using AnimalsV2;
using Model;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class AnimalRayBrain : Agent, IAgent
{
    [SerializeField] private GameObject environment;
    private SteeringAcademy environmentAcademy;
    
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;
    private float turnSpeed = 300f;
    
    
    public void Start()
    {
        animalController = GetComponent<AnimalController>();
        animalModel = animalController.animalModel;
        fsm = animalController.fsm;
        eventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        //change to a state which does not navigate the agent. If no decisionmaker is present, it will stay at this state (if default state is also set).
        fsm.SetDefaultState(animalController.idleState);
        fsm.ChangeState(animalController.idleState);
        EventSubscribe();
    }

    //Collecting observations that the ML agent should base its calculations on.
    //Choices based on https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/docs/Learning-Environment-Design-Agents.md#vector-observations
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 relativeVelocityDirection = transform.InverseTransformDirection(animalController.agent.velocity);
        sensor.AddObservation(relativeVelocityDirection.x);
        sensor.AddObservation(relativeVelocityDirection.z);
        //sensor.AddObservation(transform.InverseTransformDirection(animalController.agent.velocity));
        //sensor.AddObservation(animalModel.HungerPercentage);
        //sensor.AddObservation(animalModel.ThirstPercentage);
        //sensor.AddObservation(animalModel.WantingOffspring);

    }

    //Called Every time the ML agent decides to take an action.
    //steering inspired by: https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorAgent.cs
    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;
        int move = actions.DiscreteActions[0];
        int rotateAxis = actions.DiscreteActions[1];

        if (move == 1)
        {
            dirToGo = transform.forward;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }

        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        NavigationUtilities.NavigateRelative(animalController, dirToGo, 1 << NavMesh.GetAreaFromName("Walkable"));

    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
    }
    
    
    //Instead of updating/Making choices every frame
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        eventPublisher.onSenseTickEvent += RequestDecision;
        
        animalController.actionDeath += HandleDeath;
        animalController.eatingState.onEatFood += HandleEat;
        animalController.drinkingState.onDrinkWater += HandleDrink;
        animalController.matingState.onMate += HandleMate;
    }


    public void EventUnsubscribe()
    {
        eventPublisher.onSenseTickEvent -= RequestDecision;

        animalController.actionDeath -= HandleDeath;
        animalController.eatingState.onEatFood -= HandleEat;
        animalController.drinkingState.onDrinkWater -= HandleDrink;
        animalController.matingState.onMate -= HandleMate;

    }
    
    
    private void HandleDeath()
    {
        //Penalize for every year not lived.
        AddReward(- (1 - (animalModel.age / animalModel.traits.ageLimit)));
        //Task failed
        EndEpisode();
    }


    private void HandleDrink(GameObject water, float currentHydration)
    {
        float reward = 0f;
        if (water.gameObject.CompareTag("Water"))
        {
            // the reward should be proportional to how much hydration was gained when drinking
            reward = animalModel.traits.maxHydration - currentHydration;
            // normalize reward as a percentage
            reward /= animalModel.traits.maxHydration;
        }

        AddReward(reward);
        RequestDecision();
    }

    //The reason to why I have curentEnergy as an in-parameter is because currentEnergy is updated through EatFood before reward gets computed in AnimalMovementBrain
    private void HandleEat(GameObject food, float currentEnergy)
    {
        //Debug.Log("currentEnergy: " + animalController.animalModel.currentEnergy);
        float reward = 0f;
        //Give reward
        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal &&
            animalModel.CanEat(edibleAnimal))
        {
            float nutritionReward = edibleAnimal.nutritionValue;
            float hunger = animalModel.traits.maxEnergy - currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            // normalize reward as a percentage
            reward /= animalModel.traits.maxEnergy;
        } else if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant && animalModel.CanEat(ediblePlant))
        {
            float nutritionReward = ediblePlant.nutritionValue;
            float hunger = animalModel.traits.maxEnergy - currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            reward /= animalModel.traits.maxEnergy;
        }
        //Debug.Log("Eating reward: " +reward);
 
        AddReward(reward);
    }
    
    private void HandleMate(GameObject obj)
    {
        SetReward(2f);
        EndEpisode();
    }
    
    

    private void OnDestroy()
    {
        EndEpisode();
    }

    public Action<float> onEpisodeBegin { get; set; }
    public Action<float> onEpisodeEnd { get; set; }
}