using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using Model;
using UnityEngine;
using UnityEngine.AI;
using ViewController;

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
    private const float WalkingSpeed = 0.2f;
    private const float JoggingSpeed = 0.5f;
    private const float RunningSpeed = 1f;

    //Modifiers
    [HideInInspector] public float energyModifier;
    [HideInInspector] public float hydrationModifier;
    [HideInInspector] public float reproductiveUrgeModifier;
    [HideInInspector] public float speedModifier = JoggingSpeed; //100% of maxSpeed in model

    private bool wantsToMate;

    //target lists
    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visibleFoodTargets = new List<GameObject>();
    public List<GameObject> visibleWaterTargets = new List<GameObject>();

    public List<GameObject> heardHostileTargets = new List<GameObject>();
    public List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public List<GameObject> heardPreyTargets = new List<GameObject>();

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
                //TODO bad practice, hard coded values, this is temporary
                if (animalModel is BearModel || animalModel is WolfModel)
                    HighEnergyState();
                else
                    MediumEnergyState();
                break;
            case GoToWater _:
                MediumEnergyState();
                break;
            case GoToMate _:
                MediumEnergyState();
                break;
            case FleeingState _:
                HighEnergyState();
                break;
            case EatingState _:
                LowEnergyState();
                break;
            case DrinkingState _:
                LowEnergyState();
                break;
            case MatingState _:
                HighEnergyState();
                break;
            case SearchingState _:
                MediumEnergyState();
                break;
            case Idle _:
                LowEnergyState();
                break;
            case Wander _:
                LowEnergyState();
                break;
            case Dead _:
                energyModifier = 0f;
                hydrationModifier = 0f;
                reproductiveUrgeModifier = 0f;
                speedModifier = 0f;
                break;
        }
    }

    private void HighEnergyState()
    {
        energyModifier = 1f;
        hydrationModifier = 1f;
        reproductiveUrgeModifier = 0f;
        speedModifier = RunningSpeed;
    }
    private void MediumEnergyState()
    {
        energyModifier = 0.4f;
        hydrationModifier = 0.1f;
        reproductiveUrgeModifier = 1f;
        speedModifier = JoggingSpeed;
    }
    private void LowEnergyState()
    {
        energyModifier = 0.2f;
        hydrationModifier = 0.05f;
        reproductiveUrgeModifier = 1f;
        speedModifier = WalkingSpeed;
    }
    private void UpdateParameters()
    {
        //The age will increase 1 per 1 second.
        age += Time.deltaTime;
        
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier;
        
        // energy
        animalModel.currentEnergy -= age + 
            (animalModel.traits.viewRadius / 10 + animalModel.traits.hearingRadius / 10 + 
             currentSpeed) * animalModel.traits.size * energyModifier;
        // hydration
        animalModel.currentHydration -= animalModel.traits.size * 
                                        (1 + currentSpeed * hydrationModifier);
        // reproductive urge
        animalModel.reproductiveUrge += 0.1f * reproductiveUrgeModifier;
        
        //TODO, maybe move from here?
        agent.speed = animalModel.currentSpeed;
        
        // testing
        currentSpeed = animalModel.currentSpeed;
        maxSpeed = animalModel.traits.maxSpeed;
        isCarrying = animalModel.isCarrying;
        size = animalModel.traits.size;
        acceleration = animalModel.traits.acceleration;
        age = animalModel.age;
    }

    public float acceleration;
    public bool isCarrying;
    public float currentSpeed;
    public float maxSpeed;
    public float size;
    public float age;

    protected void EventSubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent += UpdateParameters;
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
            tickEventPublisher.onParamTickEvent -= UpdateParameters;
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
    
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
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
    void Mate(GameObject target)
    {
        wantsToMate = true;
        
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();

        System.Random rng = new System.Random();
        // random offset in case both animals have the same age
        float rnd = (float) rng.NextDouble() * 0.00001f;
        // prevent both parties from becoming pregnant by favoring the older animal as carrier.
        if (targetAnimalController.wantsToMate && 
            targetAnimalController.animalModel.age + rnd > animalModel.age)
            wantsToMate = false; 
        
        // make sure target has an AnimalController,
        // that its animalModel is same species, and neither animal is already carrying
        if (wantsToMate && targetAnimalController && 
            targetAnimalController.animalModel.IsSameSpecies(animalModel) && 
            !animalModel.isCarrying && !targetAnimalController.animalModel.isCarrying)
        {

            //TODO promote laborTime to model or something.
            float childEnergy = animalModel.currentEnergy * 0.25f +
                                targetAnimalController.animalModel.currentEnergy * 0.25f;
            // Expend energy
            animalModel.currentEnergy *= 0.75f;
            targetAnimalController.animalModel.currentEnergy *= 0.75f;
            
            // Generate the offspring's traits
            AnimalModel childModel = animalModel.Mate(rng, targetAnimalController.animalModel);
            
            // Spawn child as a copy of this parent
            GameObject child = gameObject;
            
            // Wait some time before giving birth
            animalModel.isCarrying = true;
            StartCoroutine(GiveBirth(child, childModel, childEnergy, 5));

            // Reset both reproductive urges. 
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;
        }
    }

    IEnumerator GiveBirth(GameObject child, AnimalModel childModel, float newEnergy, float laborTime)
    {
        yield return new WaitForSeconds(laborTime);
        //Instantiate here
        child = Instantiate(child, gameObject.transform.position,
            gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
        
        child.GetComponent<AnimalController>().animalModel = childModel;
        child.GetComponent<AnimalController>().animalModel.currentEnergy = newEnergy;

        // update the childs speed (in case of mutation).
        child.GetComponent<AnimalController>().animalModel.traits.maxSpeed = 1;
        
        animalModel.isCarrying = false;
        
        onBirth?.Invoke(this,new OnBirthEventArgs{child = child});
    }

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    public void DestroyGameObject(float delay)
    {
        Destroy(gameObject, delay);
    }

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
    }

    protected void Start()
    {
        // Init the NavMesh agent
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        agent.speed = animalModel.currentSpeed;


        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();

        SetPhenotype();
    }

    //should be refactored so that this logic is in AnimalModel
    private void HandleDeathStatus()
    {
        if (!animalModel.IsAlive)
        {
            //Debug.Log("Energy: "+animalModel.currentEnergy + "Hydration: "+ animalModel.currentHydration);

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
    
}