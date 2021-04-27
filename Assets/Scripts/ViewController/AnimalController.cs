using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using DefaultNamespace;
using Model;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using ViewController.Senses;
using Debug = UnityEngine.Debug;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;


public abstract class AnimalController : MonoBehaviour, IPooledObject
{

    public AnimalModel animalModel;
    public Canvas parameterUI;

    [HideInInspector] public TickEventPublisher tickEventPublisher;
    
    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;
    public Action<AnimalModel, Vector3, float, float, string> SpawnNew;

    // Start vector for the animal, used in datahandler distance travelled
    public Vector3 startVector;

    // AnimalParticleManager is subscribed to these
    public event Action<bool> ActionPregnant;

    //Subscribed to by animalBrainAgent.
    public event EventHandler<OnBirthEventArgs> OnBirth;

    public class OnBirthEventArgs : EventArgs
    {
        public GameObject child;
    }

    [HideInInspector] public NavMeshAgent agent;

    public FiniteStateMachine fsm;

    //States
    public FleeingState fleeingState;
    public GoToFood goToFoodState;
    public Wander wanderState;
    public Idle idleState;
    public GoToWater goToWaterState;
    public MatingState matingState;
    public Dead deadState;
    public DrinkingState drinkingState;
    public EatingState eatingState;
    public GoToMate goToMate;
    public Waiting waitingState;

    //Constants
    protected const float WalkingSpeed = 0.3f;
    protected const float JoggingSpeed = 0.5f;
    protected const float RunningSpeed = 1f;

    //Modifiers
    [HideInInspector] public float energyModifier;
    [HideInInspector] public float hydrationModifier;
    [HideInInspector] public float reproductiveUrgeModifier = 1f;
    [HideInInspector] public float speedModifier = JoggingSpeed; //100% of maxSpeed in model

    //Timescale stuff
    protected float baseAcceleration;
    protected float baseAngularSpeed;

    //target lists
    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visibleFoodTargets = new List<GameObject>();
    public List<GameObject> visibleWaterTargets = new List<GameObject>();


    public List<GameObject> heardHostileTargets = new List<GameObject>();
    public List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public List<GameObject> heardPreyTargets = new List<GameObject>();

    //used for ml, so that it does not spawn a lot of children that might interfere with training
    public bool isInfertile = false;

    //The neck bone of the animal.
    public Transform eyesTransform;
    public Transform centerTransform;
    
    public void Awake()
    {
        //animationController = new AnimationController(this);

        //Create the FSM.
        fsm = new FiniteStateMachine();
        
        goToFoodState = new GoToFood(this, fsm);
        fleeingState = new FleeingState(this, fsm);
        wanderState = new Wander(this, fsm);
        idleState = new Idle(this, fsm);
        goToWaterState = new GoToWater(this, fsm);
        matingState = new MatingState(this, fsm);
        deadState = new Dead(this, fsm);
        drinkingState = new DrinkingState(this, fsm);
        eatingState = new EatingState(this, fsm);
        goToMate = new GoToMate(this, fsm);
        waitingState = new Waiting(this, fsm);
        StateEventSubscribe();
        
        
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        //Can be used later.
        baseAngularSpeed = agent.angularSpeed;
        baseAcceleration = agent.acceleration;
        
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        
        if (eyesTransform == null)
        {
            Debug.LogWarning("Eyes not assigned, defaulting to transform");
            eyesTransform = transform;
        }

        if (centerTransform == null)
        {
            Debug.LogWarning("Center not assigned, defaulting to transform");
            centerTransform = transform;
        }
        
        
        //NOTE IT IS IMPORTANT THAT MODEL IS ASSIGNED (IN CONCRETE CLASS) BEFORE THIS AWAKE METHOD IS CALLED
        if (TryGetComponent(out AnimationController a))
        {
            a.Init();
        }
        if (TryGetComponent(out Senses s))
        {
            s.Init();
        }
        if (gameObject.TryGetComponent(out DecisionMaker dm))
        {
            dm.Init();
        }

    }
    

    private void Start()
    {
        //If there is no object pooler present, we need to call onObjectSpawn through start
        if (FindObjectOfType<ObjectPooler>() == null)
        {
            onObjectSpawn();
        }
    }
    
    
    /// <summary>
    /// "Start()" when using animal pooling, called when the animal is set to be active. 
    /// </summary>
    public virtual void onObjectSpawn()
    {
        
        agent.acceleration = baseAcceleration * Time.timeScale;
        agent.angularSpeed = baseAcceleration * Time.timeScale;
        
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        agent.speed = animalModel.currentSpeed * Time.timeScale;
        
        
        //agent.isStopped = false;
        fsm.Initialize(wanderState);
        //Set modifiers
        ChangeModifiers(wanderState);
        
        TickEventSubscribe();
        
        SetPhenotype();
        startVector = transform.position;
        StartCoroutine(UpdateStatesLogicLoop());
        
        if (TryGetComponent(out Senses s))
        {
            s.Activate();
        }
        if (gameObject.TryGetComponent(out DecisionMaker dm))
        {
            dm.Activate();
        }
    }
    
    public virtual void OnObjectDespawn()
    {

        if (TryGetComponent(out Senses s))
        {
            s.Deactivate();
        }
        if (TryGetComponent(out DecisionMaker dm))
        {
            dm.Deactivate();
        }  
        TickEventUnsubscribe();
        StopAllCoroutines();
    }

    public abstract string GetObjectLabel();

    private IEnumerator UpdateStatesLogicLoop()
    {
        while (true)
        {
            fsm.UpdateStatesLogic();
            yield return new WaitForSeconds(UnityRandom.Range(0.5f, 1f));
            
        }
    }

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
    public virtual void ChangeModifiers(State state)
    {
        //Debug.Log("Changing modifiers for state: " + state.ToString());
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
            case MLInferenceState _:
                energyModifier = 0.5f;
                hydrationModifier = 0.5f;
                reproductiveUrgeModifier = 20f;
                speedModifier = JoggingSpeed;
                break;
            default:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;

                speedModifier = JoggingSpeed;
                //Debug.Log("varying parameters depending on state: Wander");
                break;
        }
        //This is to make sure that the speed is set directly after State change.
        SetSpeed(speedModifier);
    }
    
    public void SetSpeed(float speedModifier)
    {
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier;
        agent.speed = animalModel.currentSpeed * Time.timeScale;
    }

    protected void HighEnergyState()
    {
        energyModifier = 1f;
        hydrationModifier = 1f;
        reproductiveUrgeModifier = 0f;
        speedModifier = RunningSpeed;
    }

    protected void MediumEnergyState()
    {
        energyModifier = 0.35f;
        hydrationModifier = 0.4f;
        reproductiveUrgeModifier = 1f;
        speedModifier = JoggingSpeed;
    }

    protected void LowEnergyState()
    {
        energyModifier = 0.15f;
        hydrationModifier = 0.15f;
        reproductiveUrgeModifier = 1.5f;
        speedModifier = WalkingSpeed;
    }

    public virtual void UpdateParameters()
    {
        //The age will increase 2 per 2 seconds.
        animalModel.age += 0.2f;

        // speed
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier;
        if (agent != null)
        {
            agent.speed = animalModel.currentSpeed * Time.timeScale;   
            agent.acceleration = baseAcceleration * Time.timeScale;
            agent.angularSpeed = baseAngularSpeed * Time.timeScale;
        }

        // energy
        animalModel.currentEnergy -= (animalModel.currentSpeed / 10 +
                                      animalModel.traits.viewRadius / 10 + animalModel.traits.hearingRadius / 8)
                                     * animalModel.traits.size * energyModifier;

        // hydration
        animalModel.currentHydration -= (animalModel.traits.size) * (1 + animalModel.currentSpeed / 10  *
                                         hydrationModifier);
        
        
        // reproductive urge
        if (animalModel.HighEnergy && animalModel.HighHydration)
        {
            animalModel.reproductiveUrge += reproductiveUrgeModifier;
        }
    }
    private void Update()
    {
        //outcommented to boost performance
        //rotateToTerrain();
    }

    //This could be used as an alternative to rotateToTerrain to avoid updating rotation every frame.
    // IEnumerator MoveObject(Vector3 source, Vector3 target, float overTime)
    // {
    //     float startTime = Time.time;
    //     while(Time.time < startTime + overTime)
    //     {
    //         transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
    //         yield return null;
    //     }S
    //     transform.position = target;
    // }
    private void rotateToTerrain()
    {
        RaycastHit hit;
        Vector3 direction = transform.TransformDirection(Vector3.down);
        Quaternion targetRotation = transform.rotation;

        if (Physics.Raycast(transform.position, direction, out hit, 50f, LayerMask.GetMask("Ground")))
        {
            Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            targetRotation = surfaceRotation * transform.rotation;
            //Dont rotate around Z.
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y,0);
        }

        transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, targetRotation,  2 * Time.deltaTime);
    }
    
    //Set animals size based on traits.
    protected virtual void SetPhenotype()
    {
        gameObject.transform.localScale = getNormalizedScale() * animalModel.traits.size;
    }

    public void EatFood(GameObject food, float currentEnergy)
    {
        if (food != null && food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal &&
            animalModel.CanEat(edibleAnimal))
        {
            animalModel.currentEnergy += edibleAnimal.GetEaten();
            
            if(food.TryGetComponent(out AnimalController eatenAnimalController)) eatenAnimalController.deadState.onDeath?.Invoke(eatenAnimalController, true);
        }

        if (food != null && food.TryGetComponent(out PlantController plantController) && plantController.plantModel is IEdible ediblePlant &&
            animalModel.CanEat(ediblePlant))
        {
            animalModel.currentEnergy += plantController.GetEaten();
        }
    }

    public void DrinkWater(GameObject water, float currentHydration)
    {
        if (water != null && water.gameObject.CompareTag("Water") && !animalModel.HydrationFull)
        {
            animalModel.currentHydration = animalModel.traits.maxHydration;
        }
    }

    void Mate(GameObject target)
    {
        if (isInfertile) return;

        AnimalController targetAnimalController = null;

        if (target != null)
        {
            targetAnimalController = target.GetComponent<AnimalController>();
        }

        if(targetAnimalController.isInfertile) return;
        
        Random rng = new Random();
        
        // make sure target has an AnimalController,
        // that its animalModel is same species, and neither animal is already carrying
        if (targetAnimalController != null && targetAnimalController.animalModel.IsSameSpecies(animalModel) &&
            targetAnimalController.animalModel.WantingOffspring)
        {

            float childEnergy = animalModel.currentEnergy * 0.3f +
                                targetAnimalController.animalModel.currentEnergy * 0.3f;
            childEnergy /= animalModel.offspringCount; // split the energy between the offspring
            float childHydration = animalModel.currentHydration * 0.25f +
                                   targetAnimalController.animalModel.currentHydration * 0.25f;
            childHydration /= animalModel.offspringCount; // split the hydration between the offspring

            // Expend energy and give it to child(ren)
            animalModel.currentEnergy *= 0.9f;
            targetAnimalController.animalModel.currentEnergy *= 0.9f;
            animalModel.currentHydration *= 0.9f;
            targetAnimalController.animalModel.currentHydration *= 0.9f;

            // Reset both reproductive urges. 
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;

            animalModel.isPregnant = true;
            ActionPregnant?.Invoke(true);
            
            for (int i = 1; i <= animalModel.offspringCount; i++)
                // Wait some time before giving birth
                StartCoroutine(GiveBirth(childEnergy, childHydration, animalModel.gestationTime, targetAnimalController));
        }
    }
    
    IEnumerator GiveBirth(float childEnergy, float childHydration, float laborTime, AnimalController otherParentAnimalController) 
    {
        yield return new WaitForSeconds(laborTime*0.6f);
        AnimalModel childModel = animalModel.Mate(otherParentAnimalController.animalModel);
        SpawnNew?.Invoke(childModel, transform.position, childEnergy, childHydration, GetObjectLabel());
        // invoke only once when birthing multiple children
        if (animalModel.isPregnant) ActionPregnant?.Invoke(false);
        animalModel.isPregnant = false;
    }

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    //Check if animal is dead and activate deadState (absorbing state)
    private void CheckDeath()
    {
        if (!animalModel.IsAlive)
        {
            if (FindObjectOfType<ObjectPooler>())
            {
                OnObjectDespawn();
            }
            fsm.ChangeState(deadState);
        }
    }
    

    public void OnDestroy()
    {
        StopAllCoroutines();
        StateEventUnSubscribe();
    }
    
    protected void TickEventSubscribe()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onParamTickEvent += UpdateParameters;
            tickEventPublisher.onParamTickEvent += CheckDeath;

        }
    }
    
    protected void TickEventUnsubscribe()
    {
        if (tickEventPublisher)
        {
            tickEventPublisher.onParamTickEvent -= UpdateParameters;
            tickEventPublisher.onParamTickEvent -= CheckDeath;
        }
    }

    private void StateEventSubscribe()
    {
        fsm.OnStateEnter += ChangeModifiers;
        eatingState.onEatFood += EatFood;
        drinkingState.onDrinkWater += DrinkWater;
        matingState.onMate += Mate;
        //animationController.EventSubscribe();
    }
    private void StateEventUnSubscribe()
    {
        fsm.OnStateEnter -= ChangeModifiers;
        eatingState.onEatFood -= EatFood;
        drinkingState.onDrinkWater -= DrinkWater;
        matingState.onMate -= Mate; 
        //animationController.EventUnsubscribe();
    }
    
    public abstract Vector3 getNormalizedScale();
    
}