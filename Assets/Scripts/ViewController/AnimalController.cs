using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using DataCollection;
using Model;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using Debug = UnityEngine.Debug;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animalModel;

    public TickEventPublisher tickEventPublisher;

    public Action<State> stateChange;

    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;
    public Action actionDeath;
    
    //Subscribed to by animalBrainAgent.
    public event EventHandler<OnBirthEventArgs> onBirth;

    public class OnBirthEventArgs : EventArgs
    {
        public GameObject child;
    }

    [HideInInspector] public NavMeshAgent agent;

    public FiniteStateMachine fsm;
    private AnimationController animationController;
    
    // Add a data handler
    private DataHandler dh;

    public enum CauseOfDeath
    {
        Hydration,
        Eaten,
        Health,
        Hunger,
    };
    
    //States
    public FleeingState fleeingState;
    public GoToFood goToFoodState;
    public Wander wanderState;
    public Idle idleState;
    public GoToWater goToWaterState;
    public MatingState matingStateState;
    public Dead deadState;
    public DrinkingState drinkingState;
    public EatingState eatingState;
    public GoToMate goToMate;

    //Constants
    private const float JoggingSpeed = 0.4f;
    private const float RunningSpeed = 1f;

    //Modifiers
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
            case GoToFood _:
                energyModifier = 0f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;

                //TODO bad practice, hard coded values, this is temporary
                if (animalModel is BearModel || animalModel is WolfModel)
                {
                    speedModifier = RunningSpeed;
                }
                else
                {
                    speedModifier = JoggingSpeed;
                }

                //Debug.Log("varying parameters depending on state: Eating");
                break;
            case FleeingState _:
                energyModifier = 1f;
                hydrationModifier = 1f;
                reproductiveUrgeModifier = 1f;
                speedModifier = RunningSpeed;
                //Debug.Log("varying parameters depending on state: FleeingState");
                break;
            case GoToWater _:
            {
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1;

                break;
            }
            case Idle _:
                energyModifier = 0f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1;
                //Debug.Log("varying parameters depending on state: Mating");
                break;
            case GoToMate _:
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
            case Dead _:
                energyModifier = 0f;
                hydrationModifier = 0f;
                reproductiveUrgeModifier = 0f;
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
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        //TODO, maybe move from here?
        agent.speed = animalModel.currentSpeed;

        animalModel.currentEnergy -= (animalModel.traits.size * 1) +
                                     (animalModel.traits.size * animalModel.currentSpeed) * energyModifier;
        animalModel.currentHydration -= (animalModel.traits.size * 1) +
                                        (animalModel.traits.size * animalModel.currentSpeed) * hydrationModifier;
        animalModel.reproductiveUrge += 0.1f * reproductiveUrgeModifier;

        //The age will increase 1 per 1 second.
        animalModel.age += Time.deltaTime;
    }

    protected void EventSubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent += VaryParameters;
            tickEventPublisher.onParamTickEvent += HandleDeathStatus;
            // every 0.5 sec
            tickEventPublisher.onSenseTickEvent += fsm.UpdateStatesLogic;    
        }
        
        fsm.OnStateEnter += ChangeModifiers;

        eatingState.onEatFood += EatFood;

        drinkingState.onDrinkWater += DrinkWater;

        matingStateState.onMate += Mate;

        animationController.EventSubscribe();
    }

    protected void EventUnsubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent -= VaryParameters;
            tickEventPublisher.onParamTickEvent -= HandleDeathStatus;
            // every 0.5 sec
            tickEventPublisher.onSenseTickEvent -= fsm.UpdateStatesLogic;    
        }
        
        
        fsm.OnStateEnter -= ChangeModifiers;

        eatingState.onEatFood -= EatFood;

        drinkingState.onDrinkWater -= DrinkWater;

        matingStateState.onMate -= Mate;

        animationController.EventUnsubscribe();
    }

    //Set animals size based on traits.
    private void SetPhenotype()
    {
        gameObject.transform.localScale = getNormalizedScale() * animalModel.traits.size;
    }

    private void EatFood(GameObject food)
    {
        //Access food script to consume the food.
        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal)
        {
            animalModel.currentEnergy += edibleAnimal.GetEaten();
            Destroy(food);
        }
        else if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant)
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
    void Mate(GameObject otherParent)
    {
        

        // make sure target has an AnimalController and that its animalModel is same species
        if (otherParent.TryGetComponent(out AnimalController otherParentAnimalController) && otherParentAnimalController.animalModel.IsSameSpecies(animalModel))
        {
            
            
            //Debug.Log(childModel.generation);
            //TODO promote laborTime to model or something.
            float childEnergy = animalModel.currentEnergy * 0.25f +
                                otherParentAnimalController.animalModel.currentEnergy * 0.25f;
            StartCoroutine(GiveBirth(gameObject, otherParentAnimalController, childEnergy, 5));

            //Reset both reproductive urges. 
            animalModel.reproductiveUrge = 0f;
            otherParentAnimalController.animalModel.reproductiveUrge = 0f;

            //Expend energy
            animalModel.currentEnergy = animalModel.currentEnergy * 0.75f;
            otherParentAnimalController.animalModel.currentEnergy = otherParentAnimalController.animalModel.currentEnergy * 0.75f;
        }
    }

    IEnumerator GiveBirth(GameObject child, AnimalController otherParentAnimalController, float newEnergy, float laborTime)
    {
        yield return new WaitForSeconds(laborTime);
        //Instantiate here
        child = Instantiate(child, gameObject.transform.position,
            gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
        
        child.GetComponent<AnimalController>().animalModel.currentEnergy = newEnergy;
        
        // Generate the offspring traits
        AnimalModel childModel = animalModel.Mate(otherParentAnimalController.animalModel);
        child.GetComponent<AnimalController>().animalModel = childModel;
        Debug.Log(child.GetComponent<AnimalController>().animalModel.generation);
        onBirth?.Invoke(this,new OnBirthEventArgs{child = child});
    }

    public void DestroyGameObject(float delay)
    {
        Destroy(gameObject, delay);
    }

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    public void Awake()
    {
        //Create the FSM.
        fsm = new FiniteStateMachine();

        goToFoodState = new GoToFood(this, fsm);
        fleeingState = new FleeingState(this, fsm);
        wanderState = new Wander(this, fsm);
        idleState = new Idle(this, fsm);
        goToWaterState = new GoToWater(this, fsm);
        matingStateState = new MatingState(this, fsm);
        deadState = new Dead(this, fsm);
        drinkingState = new DrinkingState(this, fsm);
        eatingState = new EatingState(this, fsm);
        goToMate = new GoToMate(this, fsm);
        fsm.Initialize(wanderState);

        animationController = new AnimationController(this);

        dh = FindObjectOfType<DataHandler>();
    }

    protected void Start()
    {
        // Init the NavMesh agent
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        agent.speed = animalModel.currentSpeed;

        dh.LogNewAnimal(animalModel);

        dh.LogNewAnimal(animalModel);


        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();

        SetPhenotype();
        
    }


    //should be refactored so that this logic is in AnimalModel
    private void HandleDeathStatus()
    {
        if (!animalModel.IsAlive)
        {
            CauseOfDeath cause;
            if (animalModel.currentEnergy == 0) cause = CauseOfDeath.Hunger;
            if (animalModel.currentHealth == 0) cause = CauseOfDeath.Health;
            if (animalModel.currentHydration == 0) cause = CauseOfDeath.Hydration;
            else cause = CauseOfDeath.Eaten;
            dh.LogDeadAnimal(animalModel, cause);

            // invoke death state with method HandleDeath() in decisionmaker
            actionDeath?.Invoke();
           
            // unsubscribe all events because we want only want to invoke it once.
            //actionDeath = null;
        }
    }

    public void OnDestroy()
    {
        EventUnsubscribe();
    }


    public abstract Vector3 getNormalizedScale();
    private void FixedUpdate()
    {
        //Update physics
        //Fsm.UpdateStatesPhysics();
    }
    
}