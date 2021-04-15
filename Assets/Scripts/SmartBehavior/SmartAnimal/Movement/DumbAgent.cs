using System;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using AnimalsV2;
using Model;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
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
    public Action<float> onEpisodeBegin { get; set; }
    public Action<float> onEpisodeEnd { get; set; }

    
    public void Start()
    {
        //init specific
        animalController = GetComponent<AnimalController>();
        animalModel = animalController.animalModel;
        fsm = animalController.fsm;
        eventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        //Set the animal as sterile if we want to train/heuristic
        if (TryGetComponent(out BehaviorParameters bp))
        {
            Debug.Log("Agent is infertile!");
            animalController.isInfertile =
                bp.BehaviorType == BehaviorType.Default || bp.BehaviorType == BehaviorType.HeuristicOnly;
        }
        
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
        //Get the absolute vector for all targets
        Vector3 nearestFood = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets.Concat(animalController.heardPreyTargets).ToList(), thisPosition)?.transform.position ?? thisPosition;
        Vector3 nearestWater = NavigationUtilities.GetNearestObject(animalController.visibleWaterTargets, thisPosition)?.transform.position ?? thisPosition;
        //Get the absolute vector for a potential mate
        Vector3 potentialMate = animalController.goToMate.GetFoundMate()?.transform.position ?? thisPosition;
        
        //Get Vector between animal and targets
        nearestFood = nearestFood - thisPosition;
        nearestWater = nearestWater - thisPosition;
        potentialMate = potentialMate - thisPosition;

        //Get the magnitude of nearestFood, nearestWater potentialMate. (Normalized)
        float maxPercievableDistance = animalController.animalModel.traits.viewRadius;
        float nearestFoodDistance = nearestFood.magnitude / maxPercievableDistance;
        float nearestWaterDistance = nearestWater.magnitude / maxPercievableDistance;
        float potentialMateDistance = potentialMate.magnitude / maxPercievableDistance;
        
        //Convert to relative vector to animal
        nearestFood = transform.InverseTransformDirection(nearestFood);
        nearestWater = transform.InverseTransformDirection(nearestWater);
        potentialMate = transform.InverseTransformDirection(potentialMate);
        
        //Add observations for food
        sensor.AddObservation(nearestFoodDistance);
        sensor.AddObservation(nearestFood.x);
        sensor.AddObservation(nearestFood.z);
        sensor.AddObservation(animalModel.GetEnergyPercentage);
        
        //Add observations for water
        sensor.AddObservation(nearestWaterDistance);
        sensor.AddObservation(nearestWater.x);
        sensor.AddObservation(nearestWater.z);
        sensor.AddObservation(animalModel.GetHydrationPercentage);
        
        //Add observations for mate
        sensor.AddObservation(potentialMateDistance);
        sensor.AddObservation(potentialMate.x);
        sensor.AddObservation(potentialMate.z);
        sensor.AddObservation(animalModel.WantingOffspring);
        
        
        //Add agents velocity (as a direction) to observations
        Vector3 velocity = transform.InverseTransformDirection(animalController.agent.velocity);
        sensor.AddObservation(velocity.x);
        sensor.AddObservation(velocity.z);

    }

    //Called Every time the ML agent decides to take an action.
    //steering inspired by: https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorAgent.cs
    public override void OnActionReceived(ActionBuffers actions)
    {

        AddReward(-0.0025f * 0.1f);
        Vector3 dirToGo = transform.forward;
        /*
        //binary possiblity 1 or 0
        int run = actions.DiscreteActions[0];

        //if run is 1, set running speed
        float speedModifier = run == 1 ? AnimalController.RunningSpeed : AnimalController.JoggingSpeed;
        */
        
        //Continuous actions are preclamped by mlagents [-1, 1]
        float rotationAngle = actions.ContinuousActions[0] * 110; //90
        
        //Give penalty based on how much rotation is made. 0 is no rotation and 1 (or -1) is max rotation.
        AddReward(- Math.Abs(actions.ContinuousActions[0]) * 0.0050f);
        
        //Rotate vector based on rotation from the action
        dirToGo = Quaternion.AngleAxis(rotationAngle, Vector3.up) * dirToGo;
        dirToGo *= 3;

        float speedModifier = actions.ContinuousActions[1];
        speedModifier = 0.5f * speedModifier + 0.5f; //make sure that function of interval [-1,1] maps to [0,1]
        
        
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        animalController.agent.speed = animalModel.currentSpeed * Time.timeScale;
        
        NavigationUtilities.NavigateRelative(animalController, dirToGo, 1 << NavMesh.GetAreaFromName("Walkable"));
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            continuousActions[1] = 1;
        }
        else
        {
            continuousActions[1] = -1;
        }
        

    }
    
    
    private void HandleDeath(AnimalController animalController, bool gotEaten)
    {
        AddReward(-1);
        onEpisodeEnd?.Invoke(100f);
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
        //AddReward(reward * 0.1f);
        AddReward(reward * 0.1f);
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

        AddReward(0.1f);
    }
    
    private void HandleMate(GameObject obj)
    {
        //SetReward(1f);
        AddReward(1f);
        //Task achieved
        onEpisodeEnd?.Invoke(100f);
        EndEpisode();
    }
    
    private void HandleBirth(object sender, AnimalController.OnBirthEventArgs e)
    {
        
     }
    
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        //Request decision on every sense tick
        eventPublisher.onSenseTickEvent += RequestDecision;
        animalController.deadState.onDeath += HandleDeath;
        animalController.eatingState.onEatFood += HandleEat;
        animalController.matingState.onMate += HandleMate;
        animalController.drinkingState.onDrinkWater += HandleDrink;
    }


    public void EventUnsubscribe()
    {
        eventPublisher.onSenseTickEvent -= RequestDecision;
        animalController.deadState.onDeath -= HandleDeath;
        animalController.eatingState.onEatFood -= HandleEat;
        animalController.matingState.onMate -= HandleMate;
        animalController.drinkingState.onDrinkWater -= HandleDrink;
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
/*
        if (other.gameObject.CompareTag("Wall"))
        {
            HandleDeath();
        }
        */
    }
    
}