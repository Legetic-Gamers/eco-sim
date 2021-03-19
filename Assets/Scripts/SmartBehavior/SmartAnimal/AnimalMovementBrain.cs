using AnimalsV2;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class AnimalMovementBrain : Agent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;

    


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

        eventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        //change to a state which does not navigate the agent. If no decisionmaker is precent, it will stay at this state.
        animalController.fsm.ChangeState(animalController.idleState);

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

        Vector3 thisPosition = animalController.transform.position;
        Vector3? nearestFoodPosition = NavigationUtilities.GetNearestObject(animalController.visibleFoodTargets, thisPosition)?.transform.position;
        Vector3? nearestWaterPosition = NavigationUtilities.GetNearestObject(animalController.visibleWaterTargets, thisPosition)?.transform.position;

        Vector3 nearestFoodRelativePosition = (nearestFoodPosition - thisPosition) ?? Vector3.zero;
        Vector3 nearestWaterRelativePosition = (nearestWaterPosition - thisPosition) ?? Vector3.zero;
        
        //Debug.Log(nearestFoodRelativePosition);
        //Debug.Log(nearestWaterRelativePosition);
        //Debug.Log("   ");
        
        sensor.AddObservation(animalModel.currentEnergy / animalModel.traits.maxEnergy);
        sensor.AddObservation(animalModel.currentHydration / animalModel.traits.maxHydration);
        //sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);
        //sensor.AddObservation(animalModel.reproductiveUrge / animalModel.traits.maxReproductiveUrge);
        sensor.AddObservation(nearestFoodRelativePosition);
        sensor.AddObservation(nearestWaterRelativePosition);
        //Debug.Log(sensor.ObservationSize());

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
        // AddReward(animalModel.age - animalModel.traits.ageLimit);
        // world.totalScore += (int)(animalModel.age - animalModel.traits.ageLimit);
        
        // if (world != null)
        // {
        //     world.agents.Remove(this);
        // }

        animalController.fsm.ChangeState(animalController.deadState);
        EventUnsubscribe();
        
        
        // world.SpawnNewRabbit();
        
        //Task failed
        //EndEpisode();
        
    }

    //NOTE: OnTriggerEnter needs a rigidbody + a collider with isTrigger checked to be able to trigger collisions with other colliders.
    private void OnTriggerEnter(Collider other)
    {
        //The simple logic is if we "collide with an object with a target tag" => "interact"
        if (other.gameObject.layer == LayerMask.NameToLayer("Target") && animalController)
        {
            float reward;
            reward = animalController.InteractWithReward(other.gameObject);
            if (reward > 0)
            {
                Debug.Log("reward: " + reward);
            }
            AddReward(reward);
        }
        
    }
}