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
    private const float WalkingSpeed = 0.3f;
    private const float JoggingSpeed = 0.5f;
    private const float RunningSpeed = 1f;

    //Modifiers
    [HideInInspector] public float energyModifier;
    [HideInInspector] public float hydrationModifier;
    [HideInInspector] public float reproductiveUrgeModifier;
    [HideInInspector] public float speedModifier = JoggingSpeed; //100% of maxSpeed in model

    private bool _wantsToMate;

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
        energyModifier = 0.35f;
        hydrationModifier = 0.5f;
        reproductiveUrgeModifier = 1f;
        speedModifier = JoggingSpeed;
    }
    private void LowEnergyState()
    {
        energyModifier = 0.15f;
        hydrationModifier = 0.25f;
        reproductiveUrgeModifier = 1f;
        speedModifier = WalkingSpeed;
    }
    private void UpdateParameters()
    {
        //The age will increase 2 per 2 seconds.
        animalModel.age += 2;
        
        // speed
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier;
        //TODO, maybe move from here?
        agent.speed = animalModel.currentSpeed;
        
        // energy
        animalModel.currentEnergy -= (animalModel.age + animalModel.currentSpeed + 
            animalModel.traits.viewRadius / 10 + animalModel.traits.hearingRadius / 10)
                                     * animalModel.traits.size * energyModifier;
        
        // hydration
        animalModel.currentHydration -= animalModel.traits.size * 
                                        (1 + 
                                         animalModel.currentSpeed / animalModel.traits.endurance * 
                                         hydrationModifier);
        
        // reproductive urge
        animalModel.reproductiveUrge += 0.2f * reproductiveUrgeModifier;
    }

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

    void Mate(GameObject target)
    {
        _wantsToMate = true;

        AnimalController targetAnimalController = target.GetComponent<AnimalController>();
        
        System.Random rng = new System.Random();
        // random offset in case both animals have the same age
        float rnd = (float) rng.NextDouble() * 0.00001f;
        // prevent both parties from becoming pregnant by favoring the older animal as carrier.
        if (targetAnimalController._wantsToMate && 
            targetAnimalController.animalModel.age > animalModel.age + rnd)
            _wantsToMate = false; 
        
        // make sure target has an AnimalController,
        // that its animalModel is same species, and neither animal is already carrying
        if (_wantsToMate && targetAnimalController && 
            targetAnimalController.animalModel.IsSameSpecies(animalModel) && 
            !animalModel.isPregnant && !targetAnimalController.animalModel.isPregnant)
        {
            animalModel.reproductiveUrge *= 0.1f;
            targetAnimalController.animalModel.reproductiveUrge *= 0.1f;
            
            // higher max urge gives greater potential for more offspring. (1-8 offspring)
            int offspringCount = Math.Max(1, rng.Next((int) animalModel.traits.maxReproductiveUrge / 5 + 1));
            
            // higher max urge => lower gestation time.
            float gestationTime = Mathf.Max(1, 100 / animalModel.traits.maxReproductiveUrge);
            
            // Expend energy and give it to child(ren)
            animalModel.currentEnergy *= 0.7f;
            targetAnimalController.animalModel.currentEnergy *= 0.7f;
            float childEnergy = animalModel.currentEnergy * 0.3f +
                                targetAnimalController.animalModel.currentEnergy * 0.3f;
            childEnergy /= offspringCount;
            
            for (int i = 1; i <= offspringCount; i++) 
                CreateChild(rng, targetAnimalController, gestationTime, childEnergy);
        }
    }

    void CreateChild(System.Random rng, AnimalController targetAnimalController, float gestationTime, float childEnergy)
    {
        // Generate the offspring's traits
        AnimalModel childModel = animalModel.Mate(rng, targetAnimalController.animalModel);
        
            
        // Wait some time before giving birth
        animalModel.isPregnant = true;
        StartCoroutine(GiveBirth(child, childModel, childEnergy, gestationTime));

        // Reset both reproductive urges. 
        animalModel.reproductiveUrge = 0f;
        targetAnimalController.animalModel.reproductiveUrge = 0f;
    }

    IEnumerator GiveBirth(AnimalModel childModel, float newEnergy, float laborTime)
    {
        yield return new WaitForSeconds(laborTime);
        //Instantiate here
        child = Instantiate(gameObject, transform.position,
            transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
        
        child.GetComponent<AnimalController>().animalModel = childModel;
        child.GetComponent<AnimalController>().animalModel.currentEnergy = newEnergy;

        // update the childs speed (in case of mutation).
        child.GetComponent<AnimalController>().animalModel.traits.maxSpeed = 1;
        
        animalModel.isPregnant = false;
        
        // Generate the offspring traits
        AnimalModel childModel = animalModel.Mate(otherParentAnimalController.animalModel);
        child.GetComponent<AnimalController>().animalModel = childModel;
        Debug.Log(child.GetComponent<AnimalController>().animalModel.generation);
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
    
}