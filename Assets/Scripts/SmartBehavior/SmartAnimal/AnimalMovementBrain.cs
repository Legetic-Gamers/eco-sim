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
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class AnimalMovementBrain : Agent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;
    private float turnSpeed = 300f;

    public Action<float> onEpisodeBegin;
    public Action<float> onEpisodeEnd;
    
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
        base.OnEpisodeBegin();
        onEpisodeBegin.Invoke(100f);
    }


    //Collecting observations that the ML agent should base its calculations on.
    //Choices based on https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/docs/Learning-Environment-Design-Agents.md#vector-observations
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        //Position of the animal
        Vector3 thisPosition = transform.position;
        if (animalController == null)
        {
            Debug.Log("ANIMALCONTROLLER IS NULL");
        }
        
        //Get the absolute vector for nearest food
        Vector3 nearestFoodPosition = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets.Concat(animalController.heardPreyTargets).ToList(), thisPosition)?.transform.position ?? thisPosition;
        //Get the absolute vector for neares water
        Vector3 nearestWaterPosition = NavigationUtilities.GetNearestObject(animalController.visibleWaterTargets, thisPosition)?.transform.position ?? thisPosition;;
        //Get the absolute vector for a potential mate
        Vector3 potentialMatePosition = animalController.goToMate.GetFoundMate()?.transform.position ?? thisPosition;
        
        //Convert to relative vector (based on animals position)
        nearestFoodPosition = nearestFoodPosition - thisPosition;
        nearestWaterPosition = nearestWaterPosition - thisPosition;
        potentialMatePosition = potentialMatePosition - thisPosition;
        /*


        //Normalization of vector sensor helps the model immensenly (to tighten the possible observation confirmation space)
        //https://forum.unity.com/threads/how-to-choose-observations.935741/
        nearestFoodPosition = nearestFoodPosition.normalized;
        nearestWaterPosition = nearestWaterPosition.normalized;
        potentialMatePosition = potentialMatePosition.normalized;
        
        sensor.AddObservation(animalModel.currentEnergy / animalModel.traits.maxEnergy);
        sensor.AddObservation(nearestFoodPosition);
        sensor.AddObservation(nearestFoodDistance);

        sensor.AddObservation(animalModel.currentHydration / animalModel.traits.maxHydration);        
        sensor.AddObservation(nearestWaterPosition);
        sensor.AddObservation(nearestWaterDistance);

        sensor.AddObservation(animalModel.WantingOffspring);
        sensor.AddObservation(potentialMatePosition);
        sensor.AddObservation(potentialMateDistance);        
        */
        
        // Project on plane so that we guarantee to compute an angle that represents the angle around the y-axis
        nearestFoodPosition.y = thisPosition.y;
        nearestWaterPosition.y = thisPosition.y;
        potentialMatePosition.y = thisPosition.y;
                
        // Compute angle between the animals forward-vector and the target
        float angleToNearestFood = Vector3.SignedAngle(transform.forward, nearestFoodPosition, Vector3.up)/180f;
        float angleToNearestWater = Vector3.SignedAngle(transform.forward, nearestWaterPosition, Vector3.up)/180f;
        float angleToPotentialMate = Vector3.SignedAngle(transform.forward, nearestFoodPosition, Vector3.up)/180f;
        
        
        //Get the discrete magnitude of nearestFood, nearestWater potentialMate. (Normalized)
        int nearestFoodDistance= (int)Math.Round(nearestFoodPosition.magnitude/animalModel.traits.viewRadius);
        int nearestWaterDistance = (int)Math.Round(nearestWaterPosition.magnitude/animalModel.traits.viewRadius);
        int potentialMateDistance = (int) Math.Round(potentialMatePosition.magnitude/animalModel.traits.viewRadius);

        //sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);
        //sensor.AddObservation(animalModel.reproductiveUrge / animalModel.traits.maxReproductiveUrge);
        
        sensor.AddObservation(animalModel.currentEnergy / animalModel.traits.maxEnergy);
        sensor.AddObservation(angleToNearestFood);
        sensor.AddObservation(nearestFoodDistance);
        
        sensor.AddObservation(animalModel.currentHydration / animalModel.traits.maxHydration);
        sensor.AddObservation(angleToNearestWater);
        sensor.AddObservation(nearestWaterDistance);
        
        sensor.AddObservation(animalModel.WantingOffspring);
        sensor.AddObservation(angleToPotentialMate);
        sensor.AddObservation(potentialMateDistance);


    }

    //Called Every time the ML agent decides to take an action.
    //steering inspired by: https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/Project/Assets/ML-Agents/Examples/FoodCollector/Scripts/FoodCollectorAgent.cs
    public override void OnActionReceived(ActionBuffers actions)
    {
        /*
        //hunger is the fraction of missing energy.
        float hunger = (animalModel.traits.maxEnergy - animalModel.currentEnergy)/animalModel.traits.maxEnergy;
        //thirst is the fraction of missing hydration.
        float thirst = (animalModel.traits.maxHydration - animalModel.currentHydration)/animalModel.traits.maxHydration;
        float stressFactor = -0.05f;//-0.05 means max penalty = -0.1
        
        if (animalModel.IsAlive)
        {
            //Penalize the rabbit for being hungry and thirsty. This should make the agent try to stay satiated.
            AddReward((hunger + thirst) * stressFactor);

            //Reward staying alive. If completely satiated -> 0.1. Completely drained -> 0.
            AddReward(2 * -stressFactor);
        }
        */
        
        AddReward(0.0025f);
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
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
    }
    
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        //Request decision on every sense tick
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
        onEpisodeEnd.Invoke(100f);
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

        AddReward(reward);
        RequestDecision();
    }
    
    private void HandleMate(GameObject obj)
    {
        AddReward(2f);
        EndEpisode();
        onEpisodeEnd.Invoke(100f);
    }


    private void OnDestroy()
    {
        EventUnsubscribe();
    }
}