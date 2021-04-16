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
using Debug = UnityEngine.Debug;
using Random = System.Random;
using UnityRandom = UnityEngine.Random;


public abstract class AnimalController : MonoBehaviour, IPooledObject
{

    public AnimalModel animalModel;
    public Canvas parameterUI;

    [HideInInspector] public TickEventPublisher tickEventPublisher;

    public Action<State> stateChange;

    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;
    public Action<AnimalModel, Vector3, float, float, bool> SpawnNew;

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
    public AnimationController animationController;

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
    [HideInInspector] public float reproductiveUrgeModifier = 0.3f;
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
        
        agent = GetComponent<NavMeshAgent>();
        
        
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
    public void onObjectSpawn()
    {
        animationController = new AnimationController(this);
        // Init the NavMesh agent
        agent.autoBraking = true;
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;

        //Can be used later.
        baseAngularSpeed = agent.angularSpeed;
        baseAcceleration = agent.acceleration;
        fsm.Initialize(wanderState);
        agent.speed = animalModel.currentSpeed * Time.timeScale;
        agent.acceleration *= Time.timeScale;
        agent.angularSpeed *= Time.timeScale;
        //Debug.Log(agent.autoBraking);
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();
        
        SetPhenotype();
        startVector = transform.position;
        StartCoroutine(UpdateStatesLogicLoop());
    }
    
    private IEnumerator UpdateStatesLogicLoop()
    {
        while (true)
        {
            fsm.UpdateStatesLogic();
            yield return new WaitForSeconds(UnityRandom.Range(0.5f, 1f)/Time.timeScale);
            
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
            case MLState _:
                energyModifier = 0.5f;
                hydrationModifier = 0.5f;
                reproductiveUrgeModifier = 20f;
                speedModifier = JoggingSpeed;
                break;
            default:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 0.2f;

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
        hydrationModifier = 0.5f;
        reproductiveUrgeModifier = 1f;
        speedModifier = JoggingSpeed;
    }

    protected void LowEnergyState()
    {
        energyModifier = 0.15f;
        hydrationModifier = 0.25f;
        reproductiveUrgeModifier = 1f;
        speedModifier = WalkingSpeed;
    }

    public virtual void UpdateParameters()
    {
        //The age will increase 2 per 2 seconds.
        animalModel.age += 1;

        // speed
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier;
        agent.speed = animalModel.currentSpeed * Time.timeScale;
        
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
        animalModel.reproductiveUrge += 0.01f * reproductiveUrgeModifier;
        agent.acceleration = baseAcceleration * Time.timeScale;
        agent.angularSpeed = baseAngularSpeed * Time.timeScale;
    }


    protected void EventSubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent += UpdateParameters;
            tickEventPublisher.onParamTickEvent += CheckDeath;
            // every 0.5 sec
            //tickEventPublisher.onSenseTickEvent += fsm.UpdateStatesLogic;
            
        }

        fsm.OnStateEnter += ChangeModifiers;

        eatingState.onEatFood += EatFood;

        drinkingState.onDrinkWater += DrinkWater;

        deadState.onDeath += HandleDeathStatus;

        matingState.onMate += Mate;

        animationController.EventSubscribe();
    }

    

    protected void EventUnsubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent -= UpdateParameters;
            tickEventPublisher.onParamTickEvent -= CheckDeath;
            // every 0.5 sec
            //tickEventPublisher.onSenseTickEvent -= fsm.UpdateStatesLogic;
            
        }


        if (fsm != null)
        {
            fsm.OnStateEnter -= ChangeModifiers;
        }

        eatingState.onEatFood -= EatFood;

        drinkingState.onDrinkWater -= DrinkWater;

        deadState.onDeath -= HandleDeathStatus;
        
        matingState.onMate -= Mate;

        animationController?.EventUnsubscribe();
    }


    private void Update()
    {
        rotateToTerrain();
    }

    //This could be used as an alternative to rotateToTerrain to avoid updating rotation every frame.
    // IEnumerator MoveObject(Vector3 source, Vector3 target, float overTime)
    // {
    //     float startTime = Time.time;
    //     while(Time.time < startTime + overTime)
    //     {
    //         transform.position = Vector3.Lerp(source, target, (Time.time - startTime)/overTime);
    //         yield return null;
    //     }
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
            ObjectPooler.instance?.HandleDeadAnimal(this, true);
            //Destroy(food);
        }

        if (food != null && food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant &&
            animalModel.CanEat(ediblePlant))
        {
            animalModel.currentEnergy += ediblePlant.GetEaten();
            Destroy(food);
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
            // higher max urge gives greater potential for more offspring. (1-8 offspring)
            int offspringCount = Math.Max(1, rng.Next((int) animalModel.traits.maxReproductiveUrge / 5 + 1));

            // higher max urge => lower gestation time.
            float gestationTime = Mathf.Max(1, 100 / animalModel.traits.maxReproductiveUrge);

            float childEnergy = animalModel.currentEnergy * 0.3f +
                                targetAnimalController.animalModel.currentEnergy * 0.3f;
            childEnergy /= offspringCount;
            float childHydration = animalModel.currentHydration * 0.25f +
                                   targetAnimalController.animalModel.currentHydration * 0.25f;


            // Expend energy and give it to child(ren)
            animalModel.currentEnergy *= 0.7f;
            targetAnimalController.animalModel.currentEnergy *= 0.7f;
            animalModel.currentHydration *= 0.7f;
            targetAnimalController.animalModel.currentHydration *= 0.7f;

            // Reset both reproductive urges. 
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;


            animalModel.isPregnant = true;
            ActionPregnant?.Invoke(true);
            
            for (int i = 1; i <= offspringCount; i++)
                // Wait some time before giving birth
                StartCoroutine(GiveBirth(childEnergy, childHydration, gestationTime, targetAnimalController));
        }
    }
    
    IEnumerator GiveBirth(float childEnergy, float childHydration, float laborTime, AnimalController otherParentAnimalController) 
    {
        yield return new WaitForSeconds(laborTime);
        AnimalModel childModel = animalModel.Mate(otherParentAnimalController.animalModel);
        bool isSmart = GetComponent<AnimalBrainAgent>();
        SpawnNew?.Invoke(childModel, transform.position, childEnergy, childHydration, isSmart);
        // invoke only once when birthing multiple children
        if (animalModel.isPregnant) ActionPregnant?.Invoke(false);
        animalModel.isPregnant = false;
    }

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    

    //should be refactored so that this logic is in AnimalModel
    private void HandleDeathStatus(AnimalController animalController, bool gotEaten)
    {
        //Stop animal from giving birth once dead.
        StopCoroutine("GiveBirth");
        StopAllCoroutines();
    }

    //Check if animal is dead and activate deadState (absorbing state)
    private void CheckDeath()
    {
        if (!animalModel.IsAlive)
        {
            fsm.ChangeState(deadState);
        }
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        EventUnsubscribe();
    }

    public abstract Vector3 getNormalizedScale();
    
}