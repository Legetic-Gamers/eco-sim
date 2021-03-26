using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Random = UnityEngine.Random;

public class AnimalBrainAgent : Agent,IAgent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private AnimalModel animalModel;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;


    public World world;


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
        animalController.isInfertile = true;

        eventPublisher = FindObjectOfType<global::TickEventPublisher>();

        //

        EventSubscribe();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //Reset stuff 


        //Reset animal position and rotation.
        // 
        //resetRabbit();

        ResetRabbit();

        if (fsm != null && fsm.currentState is Dead)
        {
            Destroy(gameObject);
        }

        if (world) world.ResetOnOnlyOneLeft();

        // if (StepCount == MaxStep)
        // {
        //     Destroy(gameObject);
        // }
    }

    private void ResetRabbit()
    {
        
        
        //MAKE SURE YOU ARE USING LOCAL POSITION
        if (transform != null && world != null)
        {
            transform.localPosition = world.transform.position + new Vector3(Random.Range(-world.rangeX, world.rangeX),
                0, Random.Range(-world.rangeZ, world.rangeZ));
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            Debug.Log("reset!");
            
            if (animalModel != null)
            {
                animalModel.currentEnergy = animalModel.traits.maxEnergy;
                animalModel.currentHealth = animalModel.traits.maxHealth;
                animalModel.currentHydration = animalModel.traits.maxHydration;
                animalModel.reproductiveUrge = 0.0f;
                animalModel.age = 0;
                animalController.fsm.absorbingState = false;
            }
        }
        // Destroy(animalController);
        // gameObject.AddComponent<RabbitController>();
        
        //this.animalModel = new RabbitModel();
        

    }


    //Collecting observations that the ML agent should base its calculations on.
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);


        if (animalModel == null) return;
        //parameters of the Animal = 3
        //Right now these are continous, might need to be discrete (using lowEnergy() ex.) to represent conditions.


        //Perceptions of the Animal (3 (x,y,z) * 3) = 9
        ////////////////////////////////////////////////////////////////////////////////////
        // bool foundFood = false;
        // bool foundMate = false;
        // bool foundHostile = false;
        // bool foundWater = false;

        //TODO change from just first to something smarter.
        // List<GameObject> foods = animalController?.visibleFoodTargets;
        // if (foods.Count > 0)
        // {
        //     if (foods.Any(a => a != null))
        //     {
        //         //Vector3 firstFoodPosition = firstFood.transform.position;
        //         //sensor.AddObservation(firstFoodPosition);
        //         foundFood = true;
        //     }
        // }
        //
        // List<GameObject> waters = animalController?.visibleWaterTargets;
        // if (foods.Count > 0)
        // {
        //     if (waters.Any(a => a != null))
        //     {
        //         //Vector3 firstFoodPosition = firstFood.transform.position;
        //         //sensor.AddObservation(firstFoodPosition);
        //         foundWater = true;
        //     }
        // }
        //
        //
        //TODO change from just first to something smarter. (Right now just get first heard or seen)
        // List<GameObject> mates = animalController?.visibleFriendlyTargets
        //     ?.Concat(animalController?.heardFriendlyTargets)
        //     .ToList();
        // if (mates.Count > 0)
        // {
        //     if (mates.Any(a => a != null))
        //     {
        //         // Vector3 firstMatePosition = firstMate.transform.position;
        //         // sensor.AddObservation(firstMatePosition);
        //         foundMate = true;
        //     }
        // }
        //
        //
        // if (!(animalController is null))
        // {
        //     List<GameObject> hostiles = animalController.visibleHostileTargets
        //         .Concat(animalController.heardHostileTargets)
        //         .ToList();
        //
        //     if (hostiles.Count > 0)
        //     {
        //         if (hostiles.Any(a => a != null))
        //         {
        //             // Vector3 firstHostilePosition = firstHostile.transform.position;
        //             // sensor.AddObservation(firstHostilePosition);
        //             foundHostile = true;
        //         }
        //     }
        // }


        //Also normalize
        sensor.AddObservation(animalModel.GetEnergyPercentage);
        //sensor.AddObservation(animalModel.currentSpeed / animalModel.traits.maxSpeed);
        sensor.AddObservation(animalModel.GetHydrationPercentage);
        //sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);
        //sensor.AddObservation(animalModel.WantingOffspring); IS ALREADY IN GOTOMATE REQUIREMENTS

        sensor.AddObservation(animalController.goToFoodState.MeetRequirements());
        sensor.AddObservation(animalController.goToWaterState.MeetRequirements());
        sensor.AddObservation(animalController.goToMate.MeetRequirements());
        sensor.AddObservation(animalController.fleeingState.MeetRequirements());

        /////////////////////////////////////////////////////////////////////////////////////

        //TOTAL = 6, set in "Vector Observation" of "Behavior Parameters" in inspector.
    }

    //Called Every time the ML agent decides to take an action.
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        base.OnActionReceived(vectorAction);
        ActionSegment<int> discreteActions = vectorAction.DiscreteActions;


        //hunger is the fraction of missing energy.
        float hunger = (animalModel.traits.maxEnergy - animalModel.currentEnergy) / animalModel.traits.maxEnergy;
        //thirst is the fraction of missing hydration.
        float thirst = (animalModel.traits.maxHydration - animalModel.currentHydration) /
                       animalModel.traits.maxHydration;
        // float maxLifeReward = 0.003f;
        //
        // if (animalModel.IsAlive)
        // {
        //     //Penalize the rabbit for being hungry and thirsty. This should make the agent try to stay satiated.
        //     float uncomfortPenalty = (hunger + thirst) * maxLifeReward;
        //     //Alter this to alter the reward
        //     
        //
        //     //Final reward
        //     //If completely satiated -> 0. Completely drained -> -maxLifeReward.
        //     float lifeReward = - uncomfortPenalty;
        //     
        //     
        //     //Debug.Log(lifeReward);
        //     
        //     // AddReward(lifeReward);
        //     // if(world) world.totalScore += lifeReward;
        // }

        //Debug.Log(StepCount);
        

        
        //Switch state based on action produced by ML model.
        //We are rewarding successful state changes.
        if (discreteActions[0] == 0)
        {
            ChangeState(animalController.wanderState);
            print("Wander.");
        }
        else if (discreteActions[0] == 1)
        {
            ChangeState(animalController.goToWaterState);
            print("Look for Water.");
        }
        else if (discreteActions[0] == 2)
        {
            ChangeState(animalController.goToMate);
            print("Look for Mate.");
        }
        else if (discreteActions[0] == 3)
        {
            ChangeState(animalController.goToFoodState);
            print("Look for food.");
        }


        if (animalController.fleeingState.MeetRequirements())
        {
            if (discreteActions[0] == 4)
            {
                ChangeState(animalController.fleeingState);
        
                print("Flee!");
            }
            else
            {
                AddReward(-5 / animalController.animalModel.traits.ageLimit);
                if (world) world.totalScore -= 5 / animalController.animalModel.traits.ageLimit;
            }
        }
        
        
        if (StepCount >= 1500)
        {
            EndEpisode();
            Destroy(gameObject);
        }
        
        if (animalModel.IsAlive)
        {
            //Debug.Log((hunger + thirst));
            //Lower penalty for less hunger and less thirst, but still penalty
            AddReward(-(hunger + thirst) / animalController.animalModel.traits.ageLimit);
            if (world) world.totalScore -= (hunger + thirst) / animalController.animalModel.traits.ageLimit;
        }
        
        

        // bool foundHostile = false;
        // if (!(animalController is null))
        // {
        //     List<GameObject> hostiles = animalController.visibleHostileTargets
        //         .Concat(animalController.heardHostileTargets)
        //         .ToList();
        //
        //     if (hostiles.Count > 0)
        //     {
        //         GameObject firstHostile = hostiles[0];
        //         if (hostiles.Any(a => a != null))
        //         {
        //             // Vector3 firstHostilePosition = firstHostile.transform.position;
        //             // sensor.AddObservation(firstHostilePosition);
        //             foundHostile = true;
        //         }
        //     }
        // }
        //
        // if (foundHostile && discreteActions[0] != 4)
        // {
        //     // AddReward(-0.2f);
        //     // if(world)world.totalScore -= 0.2f;
        //
        // }

        // AddReward( -0.005f);
        // world.totalScore -= 0.005f;
        //
        //Add rewards.
        // if (fsm.currentState is MatingState)
        // {
        //     // Debug.Log("Success!");
        //     // AddReward(1f);
        //     // world.totalScore += 1f;
        //     // EndEpisode();
        //     //Destroy(gameObject);
        // }
        // else if (fsm.currentState is EatingState)
        // {
        //     // float oldHunger = ((animalModel.traits.maxEnergy - previousEnergy) / animalModel.traits.maxEnergy);
        //     // AddReward(0.1f * oldHunger);
        //     // if (world) world.totalScore += 0.1f * oldHunger;
        //     
        // }else if (fsm.currentState is DrinkingState)
        // {
        //     
        //     // float oldThirst = ((animalModel.traits.maxHydration - previousHydration) / animalModel.traits.maxHydration);
        //     // AddReward(0.1f * oldThirst);
        //     // if (world) world.totalScore += 0.1f * oldThirst;
        //     
        // }else if (fsm.currentState is FleeingState)
        // {
        //     
        // }else if (fsm.currentState is Dead)
        // {
        //     // AddReward(- (1 - (animalModel.age / animalModel.traits.ageLimit)));
        //     // if (world) world.totalScore += (int)(- (1 - (animalModel.age / animalModel.traits.ageLimit)));
        //     // EndEpisode();
        //     // Destroy(gameObject);
        // }
    }

    //Used to mark an action as IMPOSIBLE.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        
        //These states cannot be exited on the fly. They need to exit on their own.
        if (fsm.currentState is FleeingState || fsm.currentState is EatingState || 
              fsm.currentState is DrinkingState|| fsm.currentState is MatingState|| fsm.currentState is Waiting)
        {
            Debug.Log("true");
            actionMask.WriteMask(0, new int[] {0, 1, 2, 3});
        }
        else
        {
            Debug.Log("False");
        }
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
        }
        else if (Input.GetKey(KeyCode.Alpha0))
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
    //Listen to when senses were updated.
    private void EventSubscribe()
    {
        eventPublisher.onSenseTickEvent += RequestDecision;

        animalController.actionDeath += HandleDeath;
        // animalController.onBirth += HandleBirth;

        animalController.matingState.onMate += HandleMate;
        animalController.eatingState.onEatFood += HandleEating;
        animalController.drinkingState.onDrinkWater += HandleDrinking;
    }


    public void EventUnsubscribe()
    {
        eventPublisher.onSenseTickEvent -= RequestDecision;

        animalController.actionDeath -= HandleDeath;
        // animalController.onBirth -= HandleBirth;

        animalController.matingState.onMate -= HandleMate;
        animalController.eatingState.onEatFood -= HandleEating;
        animalController.drinkingState.onDrinkWater -= HandleDrinking;
    }


    private void HandleDeath()
    {
        //Penalize for every year not lived. (mating gives more than death)
        // AddReward( (animalModel.age / animalModel.traits.ageLimit)/2);
        // if (world) world.totalScore += (int)((animalModel.age / animalModel.traits.ageLimit)/2);


        ChangeState(animalController.deadState);
        EventUnsubscribe();


        // world.SpawnNewRabbit();

        //Task failed
        EndEpisode();
    }

    private void HandleMate(GameObject obj)
    {
        AddReward(2f);
        if (world) world.totalScore += 2f;
        //Task achieved
        EndEpisode();
        //Destroy(gameObject);
    }

    // private void HandleBirth(object sender, AnimalController.OnBirthEventArgs e)
    // {
    //     if (world != null)
    //     {
    //         AnimalBrainAgent childAgent = e.child.GetComponent<AnimalBrainAgent>();
    //         if (childAgent != null)
    //         {
    //             world.agents.Add(childAgent);
    //         }
    //         
    //     }
    // }

    //Higher rewards for satiating thirst more. So if after the animal drank it is fully satiated
    private void HandleEating(GameObject obj, float previousEnergy)
    {
        // float oldHunger = ((animalModel.traits.maxEnergy - previousEnergy) / animalModel.traits.maxEnergy);
        // AddReward(oldHunger* oldHunger * 0.2f);
        // if (world) world.totalScore += 0.2f * oldHunger* oldHunger;
    }

    private void HandleDrinking(GameObject obj, float previousHydration)
    {
        // float oldThirst = ((animalModel.traits.maxHydration - previousHydration) / animalModel.traits.maxHydration);
        // AddReward(oldThirst * oldThirst * 0.2f);
        // if (world) world.totalScore += 0.2f * oldThirst * oldThirst;
    }

    public void Update()
    {
    }

    public Action<float> onEpisodeBegin { get; set; }
    public Action<float> onEpisodeEnd { get; set; }
}