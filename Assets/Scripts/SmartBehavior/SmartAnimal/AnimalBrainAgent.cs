using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.Priorities;

public class AnimalBrainAgent : Agent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;

    //
    private World world;


    //We could have multiple brains, EX:
    // Brain to use when no wall is present
    //public NNModel noWallBrain;
    // Brain to use when a jumpable wall is present
    //public NNModel smallWallBrain;
    // Brain to use when a wall requiring a block to jump over is present
    //public NNModel bigWallBrain;
    public void Start()
    {
        //Debug.Log("Brain Awake");

        animalController = GetComponent<AnimalController>();
        fsm = animalController.fsm;
        animalModel = animalController.animalModel;

        eventPublisher = animalController.tickEventPublisher;

        world = FindObjectOfType<World>();

        EventSubscribe();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //Reset stuff 
        

        //Reset animal position and rotation.
        // 
        //resetRabbit();
        
        //ResetRabbit();
    }

    private void ResetRabbit()
    {
        //MAKE SURE YOU ARE USING LOCAL POSITION
        transform.localPosition = new Vector3(Random.Range(-9.5f, 9.5f), 0, Random.Range(-9.5f, 9.5f));
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        Debug.Log("reset!");
        // Destroy(animalController);
        // gameObject.AddComponent<RabbitController>();
        
        //this.animalModel = new RabbitModel();
        
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

        //parameters of the Animal = 4
        //Right now these are continous, might need to be discrete (using lowEnergy() ex.) to represent conditions.
        //Also normalize
        sensor.AddObservation(animalModel.currentEnergy / animalModel.traits.maxEnergy);
        //sensor.AddObservation(animalModel.currentSpeed / animalModel.traits.maxSpeed);
        sensor.AddObservation(animalModel.currentHydration / animalModel.traits.maxHydration);
        sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);
        sensor.AddObservation(animalModel.reproductiveUrge / animalModel.traits.maxReproductiveUrge);

        //Perceptions of the Animal (3 (x,y,z) * 3) = 9
        ////////////////////////////////////////////////////////////////////////////////////
        bool foundFood = false;
        bool foundMate = false;
        bool foundHostile = false;
        //TODO change from just first to something smarter.
        List<GameObject> foods = animalController?.visibleFoodTargets;
        if (foods.Count > 0)
        {
            GameObject firstFood = foods[0];
            if (firstFood != null)
            {
                //Vector3 firstFoodPosition = firstFood.transform.position;
                //sensor.AddObservation(firstFoodPosition);
                foundFood = true;
            }
            
        }


        //TODO change from just first to something smarter. (Right now just get first heard or seen)
         List<GameObject> mates = animalController?.visibleFriendlyTargets?.Concat(animalController?.heardFriendlyTargets)
             .ToList();
         if (mates.Count > 0)
         {
             GameObject firstMate = mates[0];
             if (firstMate != null)
             {
                 // Vector3 firstMatePosition = firstMate.transform.position;
                 // sensor.AddObservation(firstMatePosition);
                 foundMate = true;
             }
         }
        
         //TODO change from just first to something smarter. (Right now just get first heard or seen)
         List<GameObject> hostiles = animalController?.visibleHostileTargets.Concat(animalController?.heardHostileTargets)
             .ToList();
        
         if (hostiles.Count > 0)
         {
             GameObject firstHostile = hostiles[0];
             if (firstHostile != null)
             {
                 // Vector3 firstHostilePosition = firstHostile.transform.position;
                 // sensor.AddObservation(firstHostilePosition);
                 foundHostile = true;
             }
         }
         
         sensor.AddObservation(foundFood);
         sensor.AddObservation(foundMate);
         sensor.AddObservation(foundHostile);
         
        /////////////////////////////////////////////////////////////////////////////////////

        //TOTAL = 7, set in "Vector Observation" of "Behavior Parameters" in inspector.
    }

    //Called Every time the ML agent decides to take an action.
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        base.OnActionReceived(vectorAction);
        ActionSegment<int> discreteActions = vectorAction.DiscreteActions;

        
        //hunger is the fraction of missing energy.
        float hunger = (animalModel.traits.maxEnergy - animalModel.currentEnergy)/animalModel.traits.maxEnergy;
        //thirst is the fraction of missing hydration.
        float thirst = (animalModel.traits.maxHydration - animalModel.currentHydration)/animalModel.traits.maxHydration;
        float stressFactor = -0.05f;//-0.05 means max penalty = -0.1

        if (animalModel.IsAlive)
        {
            //Penalize the rabbit for being hungry and thirsty. This should make the agent try to stay satiated.
            AddReward((hunger + thirst) * stressFactor);
            world.totalScore += (hunger + thirst) * stressFactor;

            //Reward staying alive. If completely satiated -> 0.1. Completely drained -> 0.
            AddReward(2 * -stressFactor);
            world.totalScore += 2 * -stressFactor;
        }


        //Switch state based on action produced by ML model.
        //We are rewarding successful state changes.
        if (discreteActions[0] == 0)
        {
            ChangeState(animalController.wanderState);
            print("Wander.");
        }
        else if (discreteActions[0] == 1)
        {
            if (ChangeState(animalController.goToWaterState))
            {
                // AddReward(0.1f);
                // world.totalScore += 0.1f;
            }
            

            print("Look for Water.");
        }
        else if (discreteActions[0] == 2)
        {
            if (ChangeState(animalController.goToMate))
            {
                // AddReward(0.1f);
                // world.totalScore += 0.1f;
                print("Wants offspring and found mate.");
            }
            
            print("Look for Mate.");
        }
        else if (discreteActions[0] == 3)
        {
            if (ChangeState(animalController.goToFoodState))
            {
                // AddReward(0.1f);
                // world.totalScore += 0.1f;
            }
            
            print("Look for food.");
        }
        else if (discreteActions[0] == 4)
        {
            if (ChangeState(animalController.fleeingState))
            {
                // AddReward(0.1f);
                // world.totalScore += 0.1f;
            }
            
            print("Flee!");
        }
    }
    
    //Used to mark an action as IMPOSIBLE.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        //actionMask.WriteMask(branch, actionIndices);
        
        //branch is the index (starting at 0) of the branch on which you want to mask the action
        //actionIndices is a list of int corresponding to the indices of the actions that the Agent cannot perform.

    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(in actionsOut);

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            print("up");
            Vector3 position = gameObject.transform.position;
            position.x = position.x + 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.x = position.x - 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("down");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.z = position.z + 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("left");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = gameObject.transform.position;
            position.z = position.z - 1f;
            NavMeshHit hit;
            NavMesh.SamplePosition(position, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
            animalController.agent.SetDestination(hit.position);
            print("right");
        }
        else if (Input.GetKey(KeyCode.Alpha1))
        {
            discreteActions[0] = 0;
            // ChangeState(animalController.goToFoodState);
            // print("Look for food.");
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[0] = 1;
            // ChangeState(animalController.goToWaterState);
            // print("Look for Water.");
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[0] = 2;
            // ChangeState(animalController.goToMate);
            // print("Look for Mate.");
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[0] = 3;
            // ChangeState(animalController.wanderState);
            // print("Wander.");
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            discreteActions[0] = 4;
            // ChangeState(animalController.fleeingState);
            // print("Flee!");
        }else if (Input.GetKey(KeyCode.Alpha0))
        {
            HandleDeath();
            // ChangeState(animalController.fleeingState);
            // print("Flee!");
        }
        
        
    }


    private bool ChangeState(State newState)
    {
        //Debug.Log(newState.ToString());
        return fsm.ChangeState(newState);
    }

    //Instead of updating/Making choices every frame
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        //eventPublisher.onParamTickEvent += MakeDecision;
        eventPublisher.onSenseTickEvent += RequestDecision;

        animalController.actionDeath += HandleDeath;
        animalController.matingStateState.onMate += HandleMate;
        animalController.eatingState.onEatFood += HandleEating;
        animalController.drinkingState.onDrinkWater += HandleDrinking;


    }

    


    public void EventUnsubscribe()
    {
        // eventPublisher.onParamTickEvent -= MakeDecision;
        eventPublisher.onSenseTickEvent -= RequestDecision;

        animalController.actionDeath -= HandleDeath;
        animalController.matingStateState.onMate -= HandleMate;
        animalController.eatingState.onEatFood -= HandleEating;
        animalController.drinkingState.onDrinkWater -= HandleDrinking;

    }

    
    private void HandleDeath()
    {
        Debug.Log("Death");
        //Penalize for every year not lived.
        // AddReward(animalModel.age - animalModel.traits.ageLimit);
        // world.totalScore += (int)(animalModel.age - animalModel.traits.ageLimit);
        
        ChangeState(animalController.deadState);
        EventUnsubscribe();
        
        
        // world.SpawnNewRabbit();
        
        //Task failed
        EndEpisode();

        //End if all rabbits are dead.
        if (world.agents.All(agent => agent.fsm.CurrentState is Dead))
        {
            Debug.Log("They all dead!");
             
             world.ResetWorld();
        }
        
        
    }
    
    private void HandleMate(GameObject obj)
    {
        AddReward(1f);
        world.totalScore += 1f;
        //Task achieved
        //EndEpisode();
    }
    
    private void HandleEating(GameObject obj)
    {
        AddReward(0.2f);
        world.totalScore += 0.2f;
    }

    private void HandleDrinking(GameObject obj)
    {
        AddReward(0.2f);
        world.totalScore += 0.2f;
    }

    public void Update()
    {
    }
}