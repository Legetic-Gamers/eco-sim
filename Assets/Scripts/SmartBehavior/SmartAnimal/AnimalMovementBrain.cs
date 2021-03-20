using System;
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

public class AnimalMovementBrain : Agent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;
    


    //We could have multiple brains, EX:
    // Brain to use when no wall is present
    //public NNModel noWallBrain;
    // Brain to use when a jumpable wall is present
    //public NNModel smallWallBrain;
    // Brain to use when a wall requiring a block to jump over is present
    //public NNModel bigWallBrain;
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

        //Reset animal position and rotation.
        //ResetRabbit();
    }

    private void ResetRabbit()
    {
        //MAKE SURE YOU ARE USING LOCAL POSITION
        transform.localPosition = new Vector3(Random.Range(-9.5f, 9.5f), 0, Random.Range(-9.5f, 9.5f));
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        Debug.Log("reset!");
        
        
        animalModel.currentEnergy = animalModel.traits.maxEnergy;
        animalModel.currentSpeed = 0;
        animalModel.currentHealth = animalModel.traits.maxHealth;
        animalModel.currentHydration = animalModel.traits.maxHydration;
        animalModel.reproductiveUrge = 0.2f;
        animalModel.age = 0;
        animalController.fsm.absorbingState = false;

    }


    //Collecting observations that the ML agent should base its calculations on.
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        //Position of the animal
        Vector3 thisPosition = animalController.transform.position;
        
        //Get the absolute vector for nearest food
        Vector3 nearestFoodPosition = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets, thisPosition)?.transform.position ?? thisPosition;;
        //Get the absolute vector for neares water
        Vector3 nearestWaterPosition = NavigationUtilities.GetNearestObject(animalController.visibleWaterTargets, thisPosition)?.transform.position ?? thisPosition;;

        //Convert to relative vector (based on animals position)
        Vector3 nearestFoodRelativePosition = nearestFoodPosition - thisPosition;
        Vector3 nearestWaterRelativePosition = nearestWaterPosition - thisPosition;

        //Get the discrete magnitude of nearestFood and nearestWater. Note: The magnitude is squared to save computational power.
        int nearestFoodMagnitude = (int)Math.Round(nearestFoodRelativePosition.sqrMagnitude);
        int nearestWaterMagnitude = (int)Math.Round(nearestWaterRelativePosition.sqrMagnitude);

        //Normalization of vector sensor helps the model immensenly (to tighten the possible observation confirmation space)
        //https://forum.unity.com/threads/how-to-choose-observations.935741/
        nearestFoodRelativePosition = Vector3.Normalize(nearestFoodRelativePosition);
        nearestWaterRelativePosition = Vector3.Normalize(nearestWaterRelativePosition);
        
        //Debug.Log("NearestFoodRelativePosition: " + nearestFoodRelativePosition.x + " " + nearestFoodRelativePosition.y + " " + nearestFoodRelativePosition.z);
        //Debug.Log("NearestWaterRelativePosition: " + nearestWaterRelativePosition.x + " " + nearestWaterRelativePosition.y + " " + nearestWaterRelativePosition.z);
        //Debug.Log("NearestFoodMagnitude squeared: " + nearestFoodMagnitude);
        //Debug.Log("NearestWaterMagnitude squeared: " + nearestWaterMagnitude);

        
        sensor.AddObservation(animalModel.currentEnergy / animalModel.traits.maxEnergy);
        sensor.AddObservation(animalModel.currentHydration / animalModel.traits.maxHydration);
        //sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);
        //sensor.AddObservation(animalModel.reproductiveUrge / animalModel.traits.maxReproductiveUrge);
        sensor.AddObservation(nearestFoodRelativePosition);
        sensor.AddObservation(nearestFoodMagnitude);
        sensor.AddObservation(nearestWaterRelativePosition);
        sensor.AddObservation(nearestWaterMagnitude);

    }

    //Called Every time the ML agent decides to take an action.
    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        
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
        
        
        float x = actions.ContinuousActions[0];
        float y = actions.ContinuousActions[1];
        float z = actions.ContinuousActions[2];
        
        //output vector for movement, next step it has to be normalized (so that it resembles a directional vector)
        Vector3 movementVector = new Vector3(x, y, z);
        
        //move the agent depending on the speed
        movementVector = movementVector.normalized * animalController.animalModel.currentSpeed;
        
        //Debug.Log(movementVector.x + "   " + movementVector.y + "   " + movementVector.z);
        
        NavigationUtilities.NavigateRelative(animalController, movementVector, 1 << NavMesh.GetAreaFromName("Walkable"));
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //base.Heuristic(in actionsOut);
        
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        /*
        if (Input.GetKey(KeyCode.UpArrow))
        {
            print("up");
            continuousActions[0] = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            print("down");
            continuousActions[0] = -1f;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            print("left");
            continuousActions[2] = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            print("right");
            continuousActions[2] = 1f;
        }
        */

    }
    //Instead of updating/Making choices every frame
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        eventPublisher.onSenseTickEvent += RequestDecision;
        
        animalController.actionDeath += HandleDeath;
        animalController.eatingState.onEatFood += HandleEat;
        animalController.drinkingState.onDrinkWater += HandleDrink;
    }


    public void EventUnsubscribe()
    {
        // eventPublisher.onParamTickEvent -= MakeDecision;
        eventPublisher.onSenseTickEvent -= RequestDecision;

        animalController.actionDeath -= HandleDeath;

    }
    
    
    private void HandleDeath()
    {
        //Penalize for every year not lived.
        AddReward(- (1 - (animalModel.age / animalModel.traits.ageLimit)));
        
        EventUnsubscribe();

        ChangeState(animalController.deadState);
        // world.SpawnNewRabbit();
        
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
        }

        if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant && animalModel.CanEat(ediblePlant))
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

    //NOTE: OnTriggerEnter needs a rigidbody + a collider with isTrigger checked to be able to trigger collisions with other colliders.
    private void OnTriggerEnter(Collider other)
    {
        //The simple logic is if we "collide with an object with a target tag" => "interact"
        if (other.gameObject.layer == LayerMask.NameToLayer("Target") && animalController)
        {
            animalController.Interact(other.gameObject);
        }
        
    }
    
    private bool ChangeState(State newState)
    {
        //Debug.Log(newState.ToString());
        return fsm.ChangeState(newState);
    }
}