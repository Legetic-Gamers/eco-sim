using System;
using System.Linq;
using AnimalsV2;
using Model;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class DumbAgent : Agent, IAgent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;
    private float turnSpeed = 3000f;
    
    public Action<float> onEpisodeBegin { get; set; }
    public Action<float> onEpisodeEnd { get; set; }

    
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
    
    public override void OnEpisodeBegin()
    {
        onEpisodeBegin?.Invoke(100f);
    }
    

    //Collecting observations that the ML agent should base its calculations on.
    //Choices based on https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/docs/Learning-Environment-Design-Agents.md#vector-observations
    public override void CollectObservations(VectorSensor sensor)
    {
        //Position of the animal
        Vector3 thisPosition = transform.position;
        //Get the absolute vector for nearest food
        Vector3 nearestFood = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets.Concat(animalController.heardPreyTargets).ToList(), thisPosition)?.transform.position ?? thisPosition;
        
        //Get Vector between animal and targets
        nearestFood = nearestFood - thisPosition;

        //Get the magnitude of nearestFood, nearestWater potentialMate. (Normalized)
        float nearestFoodDistance = nearestFood.magnitude/animalController.animalModel.traits.viewRadius;
        //Add observations
        sensor.AddObservation(transform.InverseTransformDirection(nearestFood));
        //sensor.AddObservation(Vector3.SignedAngle(transform.forward, nearestFood, Vector3.up)/180f);
        //Debug.Log(Vector3.SignedAngle(transform.forward, nearestFood, Vector3.up)/180f);
        sensor.AddObservation(nearestFoodDistance);
        sensor.AddObservation(transform.InverseTransformDirection(animalController.agent.velocity));

    }

    //Called Every time the ML agent decides to take an action.
    //steering inspired by: https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorAgent.cs
    public override void OnActionReceived(ActionBuffers actions)
    {

        AddReward(-0.0025f);
        Vector3 dirToGo = Vector3.zero;
        
        //binary possiblity 1 or 0
        int move = actions.DiscreteActions[0];
        
        //Continuous actions are preclamped by mlagents [-1, 1]
        float rotationAngle = actions.ContinuousActions[0] * 90;
        

        if (move == 1)
        {
            dirToGo = transform.forward;
        }
        
        dirToGo = Quaternion.AngleAxis(rotationAngle, Vector3.up) * dirToGo;
        
        //transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);
        NavigationUtilities.NavigateRelative(animalController, dirToGo, 1 << NavMesh.GetAreaFromName("Walkable"));
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActions[0] = 1;
        }
        else
        {
            discreteActions[0] = 0;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //represents rotation of -90 degrees
            continuousActions[0] = -0.5f;
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            //represents rotation of 90 degrees
            continuousActions[0] = 0.5f;
        }
        else
        {
            continuousActions[0] = 0;
        }
        

    }
    
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        //Request decision on every sense tick
        eventPublisher.onSenseTickEvent += RequestDecision;
        animalController.actionDeath += HandleDeath;
        animalController.eatingState.onEatFood += HandleEat;
        //animalController.drinkingState.onDrinkWater += HandleDrink;
    }


    public void EventUnsubscribe()
    {
        eventPublisher.onSenseTickEvent -= RequestDecision;
        animalController.actionDeath -= HandleDeath;
        animalController.eatingState.onEatFood -= HandleEat;
        //animalController.drinkingState.onDrinkWater -= HandleDrink;

    }
    
    
    private void HandleDeath()
    {
        //Penalize for every year not lived.
        AddReward(-1);
        onEpisodeEnd.Invoke(100f);
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

        AddReward(1f);
        onEpisodeEnd.Invoke(100f);
        EndEpisode();
    }
    
    

    private void OnDestroy()
    {
        EventUnsubscribe();
    }
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Target"))
        {
            animalController.Interact(other.gameObject);
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            HandleDeath();
        }
    }
    
}