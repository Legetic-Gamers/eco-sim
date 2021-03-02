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
    [SerializeField] private GameObject world;
    

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
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        //Reset stuff 
        //MAKE SURE YOU ARE USING LOCAL POSITION
        // transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
        // transform.localPosition = new Vector3(Random.Range(-4.5f,0f),0,Random.Range(-3f,1.3f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        // sensor.AddObservation(transform.localPosition);
        // sensor.AddObservation(targetTransform.localPosition);
        //
        
        
        //parameters of the Animal = 5
        sensor.AddObservation(animalModel.currentEnergy);
        sensor.AddObservation(animalModel.currentSpeed);
        sensor.AddObservation(animalModel.currentHydration);
        sensor.AddObservation(animalModel.currentHealth);
        sensor.AddObservation(animalModel.reproductiveUrge);
        
        
        //Perceptions of the Animal (3 (x,y,z) * 3) = 9
        ////////////////////////////////////////////////////////////////////////////////////
        //TODO change from just first to something smarter.
        GameObject firstFood = animalController?.visibleFoodTargets[0];
        if (firstFood != null)
        {
            Vector3 firstFoodPosition = firstFood.transform.position;
            sensor.AddObservation(firstFoodPosition);
        }
        
        //TODO change from just first to something smarter. (Right now just get first heard or seen)
        GameObject firstMate = animalController?.visibleFriendlyTargets.Concat(animalController?.heardFriendlyTargets).ToList()[0];
        if (firstMate != null)
        {
            Vector3 firstMatePosition = firstMate.transform.position;
            sensor.AddObservation(firstMatePosition);
        }
        
        //TODO change from just first to something smarter. (Right now just get first heard or seen)
        GameObject firstHostile = animalController?.visibleHostileTargets.Concat(animalController?.heardHostileTargets).ToList()[0];
        if (firstHostile != null)
        {
            Vector3 firstHostilePosition = firstHostile.transform.position;
            sensor.AddObservation(firstHostilePosition);
        }
        /////////////////////////////////////////////////////////////////////////////////////
        
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        // float moveX = vectorAction[0];
        // float moveZ = vectorAction[1];
        //
        // float moveSpeed = 10f;
        // transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        PerformBestAction(vectorAction);
    }

    //TESTING
    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);
        // float[] continousActions = actionsOut;
        // continousActions[0] = Input.GetAxisRaw("Horizontal");
        // continousActions[1] = Input.GetAxisRaw("Vertical");
    }

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


    /// <summary>
    /// Makes a decision based on the senses and the parameters of the animal (its perception of itself and its environment)
    /// to make a decision of an action to take.
    /// This action will most likely reslult in a new state as parameters/location/environment will have changed
    /// du to the action.
    /// 
    /// </summary>
    /// <returns></returns>
    // private void MakeDecision()
    // {
    //     //TODO STATE should be called ACTION instead?!
    //     GetBestAction(animalModel);
    // }

    private void PerformBestAction(float[] vectorAction)
    {
        //Parse the brain action results (which action is most wanted by the brain?)
        //TODO
        
        

        //
        State currentState = fsm.CurrentState;

        switch (currentState)
        {
            case Eating state:
            {
                Eating eatingState = state;
                //Eat until full or out of food.
                if (eatingState.foodIsEmpty() || animalModel.EnergyFull)
                {
                    //Change to some other state
                }

                break;
            }
            case Drinking state:
            {
                Drinking drinkingState = state;
                //Change to some other state
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
                    //Change to some other state  
                }

                break;
            }
            case Idle _:
                //Change to some other state
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
                    //Change to some other state
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
                    //Change to some other state
                }

                break;
            }
            case Mating _:
                //Change to some other state
                break;
        }
    }


    private void ChangeState(State newState)
    {
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
        ChangeState(animalController.deadState);
    }
}