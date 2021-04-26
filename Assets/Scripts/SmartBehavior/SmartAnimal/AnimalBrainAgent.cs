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
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.Priorities;
using Random = UnityEngine.Random;

public class AnimalBrainAgent : Agent,IAgent
{
    //ANIMAL RELATED THINGS
    private AnimalController animalController;
    private TickEventPublisher eventPublisher;
    private FiniteStateMachine fsm;


    public World world;

    public void Init()
    {
        animalController = GetComponent<AnimalController>();
        fsm = animalController.fsm;
        eventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();
    }

    public void OnDestroy()
    {
        EventUnsubscribe();
    }

    public void Activate()
    {
        eventPublisher.onSenseTickEvent += RequestDecision;
    }

    public void Deactivate()
    {
        eventPublisher.onSenseTickEvent -= RequestDecision;
    }

    public override void OnEpisodeBegin()
    {

        //dont run OnEpisodeBegin if inference mode
        if (TryGetComponent(out BehaviorParameters bp) && bp.BehaviorType == BehaviorType.InferenceOnly) return;
        
        
        Debug.Log("WHY IS ONEPISODEBEGIN GETTING CALLED?");
        base.OnEpisodeBegin();
        

        //Reset animal position and rotation.
        ResetRabbit();

        if (fsm != null && fsm.currentState is Dead)
        {
            Destroy(gameObject);
        }

        if (world) world.ResetOnOnlyOneLeft();

        
    }

    private void ResetRabbit()
    {
        AnimalModel animalModel = animalController.animalModel;
        //MAKE SURE YOU ARE USING LOCAL POSITION
        if (transform != null && world != null)
        {
            transform.localPosition = world.transform.position + new Vector3(Random.Range(-world.rangeX, world.rangeX),
                0, Random.Range(-world.rangeZ, world.rangeZ));
            transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            Debug.Log("reset!");
            
            if (animalModel != null && animalController !=null)
            {
                float beginEnergy = Random.Range(0.3f, 1f);
                float beginHydration = Random.Range(0.3f, 1f);

                animalModel.currentEnergy = animalModel.traits.maxEnergy * beginEnergy;
                animalModel.currentHealth = animalModel.traits.maxHealth;
                animalModel.currentHydration = animalModel.traits.maxHydration * beginHydration;
                animalModel.reproductiveUrge = 0.0f;
                animalModel.age = 0;
                animalController.fsm.absorbingState = false;
                
                animalController.isInfertile = true;
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
        
        AnimalModel animalModel = animalController.animalModel;

        if (animalModel == null) return;
        
        //parameters of the Animal = 2
        sensor.AddObservation(animalModel.GetEnergyPercentage);
        //sensor.AddObservation(animalModel.currentSpeed / animalModel.traits.maxSpeed); //UNESSECARY
        sensor.AddObservation(animalModel.GetHydrationPercentage); 
        //sensor.AddObservation(animalModel.currentHealth / animalModel.traits.maxHealth);//UNESSECARY
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
        
        AnimalModel animalModel = animalController.animalModel;

        ActionSegment<int> discreteActions = vectorAction.DiscreteActions;

        
        //Switch state based on action produced by ML model.
        //We are rewarding successful state changes.
        if (discreteActions[0] == 0)
        {
            ChangeState(animalController.wanderState);
            //print("Wander.");
        }
        else if (discreteActions[0] == 1)
        {
            ChangeState(animalController.goToWaterState);
            //print("Look for Water.");
        }
        else if (discreteActions[0] == 2)
        {
            ChangeState(animalController.goToMate);
            //print("Look for Mate.");
        }
        else if (discreteActions[0] == 3)
        {
            ChangeState(animalController.goToFoodState);
            //print("Look for food.");
        }

        ///////////////////////////////////////////////////////////////SET REWARDS/PENALTIES

        if (animalController.fleeingState.MeetRequirements())
        {
            if (discreteActions[0] == 4)
            {
                ChangeState(animalController.fleeingState);
        
                //print("Flee!");
            }
            else
            {
                AddReward(-1);
                if (world) world.totalScore -= 1;
            }
        }
        
        //hunger is the fraction of missing energy.
        float hunger = (animalModel.traits.maxEnergy - animalModel.currentEnergy) / animalModel.traits.maxEnergy;
        //thirst is the fraction of missing hydration.
        float thirst = (animalModel.traits.maxHydration - animalModel.currentHydration) /
                       animalModel.traits.maxHydration;
        
        if (animalModel.IsAlive)
        {
            //Debug.Log((hunger + thirst));
            //Lower penalty for less hunger and less thirst, but still penalty
            AddReward(-(hunger + thirst) / animalController.animalModel.traits.ageLimit);
            if (world) world.totalScore -= (hunger + thirst) / animalController.animalModel.traits.ageLimit;
        }
        
        
        //////////////////////////////////////////MAX Count functionality
        if (StepCount >= 1500)
        {
            
            //TODO REENABLE FOR TRAINING
            //EndEpisode();
            
        }
        
    }

    //Used to mark an action as IMPOSIBLE.
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        
        //These states cannot be exited on the fly. They need to exit on their own.
        if (fsm.currentState is FleeingState || fsm.currentState is EatingState || 
              fsm.currentState is DrinkingState|| fsm.currentState is MatingState|| fsm.currentState is Waiting)
        {
            
            actionMask.WriteMask(0, new int[] {0, 1, 2, 3});
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
            
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            discreteActions[0] = 1;
            
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            discreteActions[0] = 2;
            
        }
        else if (Input.GetKey(KeyCode.Alpha4))
        {
            discreteActions[0] = 3;
            
        }
        else if (Input.GetKey(KeyCode.Alpha5))
        {
            discreteActions[0] = 4;
            
        }
        /*
        else if (Input.GetKey(KeyCode.Alpha0))
        {
            HandleDeath();
            
        }
        */
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
        animalController.deadState.onDeath += HandleDeath;
        animalController.matingState.onMate += HandleMate;
        animalController.eatingState.onEatFood += HandleEating;
        animalController.drinkingState.onDrinkWater += HandleDrinking;
        
        //For fast action on spotting hostile
        animalController.actionPerceivedHostile += HandleHostileTarget;
    }


    public void EventUnsubscribe()
    {
        animalController.deadState.onDeath -= HandleDeath;
        animalController.matingState.onMate -= HandleMate;
        animalController.eatingState.onEatFood -= HandleEating;
        animalController.drinkingState.onDrinkWater -= HandleDrinking;
        
        //For fast action on spotting hostile
        animalController.actionPerceivedHostile -= HandleHostileTarget;
    }

    //Called when the animal sees a hostile animal. This allows for quicker reaction times to this urgent event.
    private void HandleHostileTarget(GameObject obj)
    {
        RequestDecision();
    }


    private void HandleDeath(AnimalController animalController, bool gotEaten)
    {
        //Penalize for every year not lived. (mating gives more than death)
        // AddReward( (animalModel.age / animalModel.traits.ageLimit)/2);
        // if (world) world.totalScore += (int)((animalModel.age / animalModel.traits.ageLimit)/2);
        
        //ChangeState(animalController.deadState);
        EventUnsubscribe();

        //Task failed
        //TODO REENABLE FOR TRAINING
        //EndEpisode();
    }

    private void HandleMate(GameObject obj)
    {
        AddReward(2f);
        if (world) world.totalScore += 2f;
        
        //Task achieved
        //TODO REENABLE FOR TRAINING
        //EndEpisode();

    }

   
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