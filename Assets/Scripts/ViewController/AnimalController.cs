using System;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using DataCollection;
using Model;
using UnityEngine;
using UnityEngine.AI;
using ViewController;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animalModel;

    private TickEventPublisher tickEventPublisher;
    private DataHandler datahandler;

    public Action<State> stateChange;
    
    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;
    public Action actionDeath;
    
    public Action<AnimalModel> onBirth;
    
    [HideInInspector]
    public NavMeshAgent agent;
    
    public FiniteStateMachine fsm;
    private AnimationController animationController;
    private DecisionMaker decisionMaker;
    
    /*public GoToMate sm;
    public GoToFood sf;
    public GoToWater sw;*/
    public FleeingState fleeingState;
    public Eating eatingState;
    public Wander wanderState;
    public GoToState goToState;
    public Idle idleState;
    public Drinking drinkingState;
    public Mating matingState;
    public Dead deadState;

    //Constants
    private const float JoggingSpeed = 0.4f;
    private const float RunningSpeed = 1f;

    private float energyModifier;
    private float hydrationModifier;
    private float reproductiveUrgeModifier;
    private float speedModifier = JoggingSpeed; //100% of maxSpeed in model
    
    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visibleFoodTargets = new List<GameObject>();
    public List<GameObject> visibleWaterTargets = new List<GameObject>();
    
    public List<GameObject> heardHostileTargets = new List<GameObject>();
    public List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public List<GameObject> heardPreyTargets = new List<GameObject>();

    public bool IsControllable { get; set; } = false;

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                   Parameter handlers                                   */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    /// <summary>
    /// Parameter levels are to constantly be ticking down.
    /// Using tickEvent so as to not need a separate yielding tick thread for each animal.
    /// Instead just use one global event publisher that handles the ticking,
    /// then on each tick decrement each of the meters.
    ///
    /// Important to unsubscribe from the event publisher on death, however!
    /// </summary>

    private void ChangeModifiers(State state)
    {
        switch (state)
        {
            case Eating _:
                energyModifier = 0f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;
                //Debug.Log("varying parameters depending on state: Eating");
                break;
            case FleeingState _:
                energyModifier = 1f;
                hydrationModifier = 1f;
                reproductiveUrgeModifier = 1f;
                speedModifier = RunningSpeed;
                //Debug.Log("varying parameters depending on state: FleeingState");
                break;
            case GoToState toState:
            {
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1;

                GoToState chaseState = toState;
                GameObject target = chaseState.GetTarget();
            
                if (target != null)
                {
                    AnimalController targetController = target.GetComponent<AnimalController>();
                    //target is an animal and i can eat it -> we are chasing something.
                    if (targetController != null && animalModel.CanEat(targetController.animalModel)){
                        //Run fast if chasing
                        speedModifier = RunningSpeed;
                    
                    }
                }
                //Debug.Log("varying parameters depending on state: GoToFood");
                break;
            }
            case Idle _:
                energyModifier = 0f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1;
                //Debug.Log("varying parameters depending on state: Mating");
                break;
            case Mating _:
                energyModifier = 0.2f;
                hydrationModifier = 0.2f;
                reproductiveUrgeModifier = 0f;
                //Debug.Log("varying parameters depending on state: Wander");
                break;
            case Wander _:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;
            
                speedModifier = JoggingSpeed;
                //Debug.Log("varying parameters depending on state: Wander");
                break;
        }
    }

    private void VaryParameters()
    {
        /*
         * - Size * (deltaTemp / tempResist) * Const
         * - (Vision + Hearing + Smell) * Const
         * - currentAge * Const
         *
         * when highEnergy (can be 0 or 1), also add:
         * - Size * Speed * Const
         *
         * currentEnergy -= ( size * (deltaTemp / tempResist) + (vision + hearing + smell) + currentAge
         *                  + (highEnergy * size * speed) ) * Const
         */

        //https://www.uvm.edu/pdodds/research/papers/others/2017/hirt2017a.pdf
        //above link for actual empirical max speed.
        //
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        //TODO, maybe move from here?
        agent.speed = animalModel.currentSpeed;

        animalModel.currentEnergy -= (animalModel.traits.size * 1) + (animalModel.traits.size * animalModel.currentSpeed) * energyModifier;
        animalModel.currentHydration -= (animalModel.traits.size * 1) + (animalModel.traits.size * animalModel.currentSpeed) * hydrationModifier;
        animalModel.reproductiveUrge += 0.1f * reproductiveUrgeModifier;
        //animalModel.age++;
        
    }

    protected void EventSubscribe()
    {
        // every 2 sec
        tickEventPublisher.onParamTickEvent += VaryParameters;
        tickEventPublisher.onParamTickEvent += HandleDeathStatus;
        // every 0.5 sec
        tickEventPublisher.onSenseTickEvent += fsm.UpdateStatesLogic;
        
        fsm.OnStateEnter += ChangeModifiers;

        eatingState.onEatFood += EatFood;

        drinkingState.onDrinkWater += DrinkWater;

        matingState.onMate += Mate;
        
        animationController.EventSubscribe();
    }
    protected void EventUnsubscribe()
    {
        // every 2 sec
        tickEventPublisher.onParamTickEvent -= VaryParameters;
        tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
        // every 0.5 sec
        tickEventPublisher.onSenseTickEvent -= fsm.UpdateStatesLogic;

        fsm.OnStateEnter -= ChangeModifiers;
        
        eatingState.onEatFood -= EatFood;

        drinkingState.onDrinkWater -= DrinkWater;

        matingState.onMate -= Mate;
        
        animationController.EventUnsubscribe();
        decisionMaker.EventUnsubscribe();
    }
    
    //Set animals size based on traits.
    private void SetPhenotype()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1) * animalModel.traits.size;
    }

    private void EatFood(GameObject food)
    {
        
        //Access food script to consume the food.
        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal)
        {
            animalModel.currentEnergy += edibleAnimal.GetEaten();
            Destroy(food);
            
        }else if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant)
        {
            animalModel.currentEnergy += ediblePlant.GetEaten();
            Destroy(food);
        }
    }

    private void DrinkWater(GameObject water)
    {
        if (water.gameObject.CompareTag("Water"))
        {
            animalModel.currentHydration = animalModel.traits.maxHydration;
        }
    }
    
    //TODO a rabbit should be able to have more than one offspring at a time
    void Mate(GameObject target)
    {

        AnimalController targetAnimalController = target.GetComponent<AnimalController>();
        
        // make sure target has an AnimalController and that its animalModel is same species
        if (targetAnimalController != null && targetAnimalController.animalModel.IsSameSpecies(animalModel))
        {
            
            // Spawn child as a copy of the father at the position of the mother
            GameObject child = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
            // Generate the offspring traits
            AnimalModel childModel = animalModel.Mate(targetAnimalController.animalModel);
            child.GetComponent<AnimalController>().animalModel = childModel;
            //childModel.traits.size = 1f;

            //Debug.Log("MATE");
            
            //Reset both reproductive urges.
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;
        }
    }

    public void DestroyGameObject(float delay)
    {
        Destroy(gameObject, delay);
    }
    
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    protected void Start()
    {
        // Init the NavMesh agent
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        agent.speed = animalModel.currentSpeed;

        //Create the FSM.
        fsm = new FiniteStateMachine();
        animationController = new AnimationController(this);
        
        eatingState = new Eating(this, fsm);
        fleeingState = new FleeingState(this, fsm);
        wanderState = new Wander(this, fsm);
        goToState = new GoToState(this, fsm);
        idleState = new Idle(this, fsm);
        drinkingState = new Drinking(this, fsm);
        matingState = new Mating(this, fsm);
        deadState = new Dead(this, fsm);
        fsm.Initialize(idleState);
        
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();
        
        // Could be done via events...
        datahandler = FindObjectOfType<DataHandler>();
        datahandler.LogNewAnimal(animalModel);
        
        SetPhenotype();
        
        decisionMaker = new DecisionMaker(this,animalModel,tickEventPublisher);
        onBirth?.Invoke(animalModel);
    }


    //should be refactored so that this logic is in AnimalModel
    private void HandleDeathStatus()
    {
        if (!animalModel.IsAlive)
        {
            // invoke death state with method HandleDeath() in decisionmaker
            actionDeath?.Invoke();
            // Set state so that it can't change
            fsm.absorbingState = true;
            // unsubscribe all events because we want only want to invoke it once.
            actionDeath = null;
        }
    }

    public void OnDestroy()
    {
        EventUnsubscribe();
    }

    private void FixedUpdate()
    {
        //Update physics
        //Fsm.UpdateStatesPhysics();
    }
    
    
}
