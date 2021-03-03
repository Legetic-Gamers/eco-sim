using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
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
    public override void Initialize()
    {
        animalController = GetComponent<AnimalController>();
        fsm = animalController.fsm;
        animalModel = animalController.animalModel;
        //this.eventPublisher = eventPublisher;

        world = FindObjectOfType<World>();
        
        EventSubscribe();
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //Reset stuff 
        //MAKE SURE YOU ARE USING LOCAL POSITION

        //Reset animal position and rotation.
        transform.localPosition = new Vector3(Random.Range(-9.5f, 9.5f), 0, Random.Range(-9.5f, 9.5f));
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        world.ResetWorld();
        //transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
    }

    //Collecting observations that the ML agent should base its calculations on.
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        //parameters of the Animal = 5
        //Right now these are continous, might need to be discrete (using lowEnergy() ex.) to represent conditions.
        sensor.AddObservation(animalModel.currentEnergy);
        sensor.AddObservation(animalModel.currentSpeed);
        sensor.AddObservation(animalModel.currentHydration);
        sensor.AddObservation(animalModel.currentHealth);
        sensor.AddObservation(animalModel.reproductiveUrge);


        //Perceptions of the Animal (3 (x,y,z) * 3) = 9
        ////////////////////////////////////////////////////////////////////////////////////
        //TODO change from just first to something smarter.
        // GameObject firstFood = animalController?.visibleFoodTargets?[0];
        // if (firstFood != null)
        // {
        //     Vector3 firstFoodPosition = firstFood.transform.position;
        //     sensor.AddObservation(firstFoodPosition);
        // }
        //
        // //TODO change from just first to something smarter. (Right now just get first heard or seen)
        // List<GameObject> mates = animalController?.visibleFriendlyTargets?.Concat(animalController?.heardFriendlyTargets)
        //     .ToList();
        // if (mates.Count > 0)
        // {
        //     GameObject firstMate = mates[0];
        //     if (firstMate != null)
        //     {
        //         Vector3 firstMatePosition = firstMate.transform.position;
        //         sensor.AddObservation(firstMatePosition);
        //     }
        // }
        //
        // //TODO change from just first to something smarter. (Right now just get first heard or seen)
        // List<GameObject> hostiles = animalController?.visibleHostileTargets.Concat(animalController?.heardHostileTargets)
        //     .ToList();
        //
        // if (hostiles.Count > 0)
        // {
        //     GameObject firstHostile = hostiles[0];
        //     if (firstHostile != null)
        //     {
        //         Vector3 firstHostilePosition = firstHostile.transform.position;
        //         sensor.AddObservation(firstHostilePosition);
        //     }
        // }
        /////////////////////////////////////////////////////////////////////////////////////

        //TOTAL = 14, set in "Vector Observation" of "Behavior Parameters" in inspector.
    }

    //Called Every time the ML agent decides to take an action.
    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        PerformBestAction(vectorAction);
    }

    //Used for testing, gives us control over the output from the ML algortihm.
    public override void Heuristic(float[] actionsOut)
    {
        //Change search prio based on key input.
        for (int i = 0; i < 10; ++i)
        {
            if (Input.GetKey("" + i))
            {
                actionsOut[0] = Mathf.Clamp(i, 0, 2);
            }
        }
    }

    //To set reward we could use biological fitness = fertile ofspring produced = Good reward
    //Penalty could be set upon death. Perhaps based on how many fertile offspring produced in lifetime and
    //how long the rabbit lived?

    private void OnTriggerEnter(Collider other)
    {
        // if (other.TryGetComponent<Goal>(out Goal goal))
        // {
        //     SetReward(1f);
        //     floorMeshRenderer.material = winMaterial;
        //     EndEpisode();
        // }
        //
        // if (other.TryGetComponent<Wall>(out Wall wall))
        // {
        //     SetReward(-1f);
        //     floorMeshRenderer.material = loseMaterial;
        //     EndEpisode();
        // }
    }

    List<Priorities> prio = new List<Priorities>();

    /// <summary>
    /// Set constraints based on current State.
    /// 
    /// </summary>
    /// <returns></returns>
    private void PerformBestAction(float[] vectorAction)
    {
        State currentState = fsm?.CurrentState;

        //Debug.Log(currentState);

        switch (currentState)
        {
            case Eating state:
            {
                Eating eatingState = state;
                //Eat until full or out of food.
                if (eatingState.foodIsEmpty() || animalModel.EnergyFull)
                {
                    Prioritize(vectorAction);
                }

                break;
            }
            case Drinking state:
            {
                Drinking drinkingState = state;
                Prioritize(vectorAction);
                break;
            }
            case FleeingState state:
            {
                //Run until no predator nearby.
                //Run a bit longer?

                FleeingState fleeingState = state;
                if (fleeingState.HasFled())
                {
                    //if we arrive here there the animal has sucessfully fled
                    Prioritize(vectorAction);
                }

                break;
            }
            case Idle _:
                Prioritize(vectorAction);
                break;
            case GoToState state:
            {
                GoToState goToState = state;
                if (goToState.GetTarget() != null && goToState.GetAction() != null)
                {
                    if (goToState.arrivedAtTarget())
                    {
                        GameObject target = goToState.GetTarget();
                        Priorities actionToDo = goToState.GetAction();

                        switch (actionToDo)
                        {
                            case Food:
                                animalController.eatingState.EatFood(target);
                                ChangeState(animalController.eatingState);
                                break;
                            case Water:
                                animalController.drinkingState.DrinkWater(target);
                                ChangeState(animalController.drinkingState);
                                break;
                            case Mate:
                                animalController.matingState.Mate(target);
                                ChangeState(animalController.drinkingState);
                                break;
                        }
                    }
                }
                else
                {
                    Prioritize(vectorAction);
                }

                break;
            }
            case Wander state:
            {
                Wander wander = state;
                var targetAndAction = wander.FoundObject();
                if (targetAndAction?.Item1 != null)
                {
                    animalController.goToState.SetTarget(targetAndAction.Item1);
                    animalController.goToState.SetActionOnArrive(targetAndAction.Item2);
                    ChangeState(animalController.goToState);
                }
                else
                {
                    //If no food found, try to reprioritize the search.
                    Prioritize(vectorAction);
                }

                break;
            }
            case Mating _:
            {
                Prioritize(vectorAction);
                break;
            }
        }
    }

    /// <summary>
    /// Prioritizes what to look for based on the ML Brain.
    /// </summary>
    /// <param name="vectorAction"></param>
    private void Prioritize(float[] vectorAction)
    {
        //Parse the brain action results (which action is most wanted by the brain?)
        //0 = search for food, 1 = water, 2 = mate.
        //TODO this is not complete probably
        switch (vectorAction[0])
        {
            case 0:
                //Reinsert food at top.
                prio.Remove(Food);
                prio.Insert(0, Food);

                break;
            case 1:
                //Reinsert Water at top.
                prio.Remove(Water);
                prio.Insert(0, Water);
                break;
            case 2:
                //Reinsert Mate at top.
                prio.Remove(Mate);
                prio.Insert(0, Mate);
                break;
        }

        Debug.Log(prio[0].ToString());
        animalController.wanderState.SetPriorities(prio);
        ChangeState(animalController.wanderState);
    }

    private void ChangeState(State newState)
    {
        //Debug.Log(newState.ToString());
        fsm.ChangeState(newState);
    }

    //Instead of updating/Making choices every frame
    //Listen to when parameters or senses were updated.
    private void EventSubscribe()
    {
        // eventPublisher.onParamTickEvent += MakeDecision;
        // eventPublisher.onSenseTickEvent += MakeDecision;

        animalController.actionPerceivedHostile += HandleHostileTarget;
        animalController.actionDeath += HandleDeath;
    }


    public void EventUnsubscribe()
    {
        // eventPublisher.onParamTickEvent -= MakeDecision;
        // eventPublisher.onSenseTickEvent -= MakeDecision;

        animalController.actionPerceivedHostile -= HandleHostileTarget;
        animalController.actionDeath -= HandleDeath;
    }

    /// <summary>
    /// Handle perceived target such that GetBestAction can then use it in deciding an action.
    /// </summary>
    /// <param name="target"> perceived target sent from either FieldOfView or HearingAbility,
    /// which method that will be called depends on the type of target </param>
    private void HandleHostileTarget(GameObject target)
    {
        ChangeState(animalController.fleeingState);
    }

    private void HandleDeath()
    {
        int offspringConstant = 20;
        
        AddReward(animalModel.nProducedOffspring * offspringConstant  + animalModel.age);
        ChangeState(animalController.deadState);
        
        EndEpisode();
        

        EventUnsubscribe();
    }
}