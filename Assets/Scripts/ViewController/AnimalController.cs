using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using JetBrains.Annotations;
using Model;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using Object = UnityEngine.Object;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animalModel;

    [HideInInspector] public TickEventPublisher tickEventPublisher;

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
    public MatingState matingState;
    public Dead deadState;
    public DrinkingState drinkingState;
    public EatingState eatingState;
    public GoToMate goToMate;
    public Waiting waitingState;

    //Constants
    public const float JoggingSpeed = 0.4f;
    public const float RunningSpeed = 1f;
    
    //Timescale stuff
    private float baseAcceleration;
    private float baseAngularSpeed;

    //Modifiers
    private float energyModifier;
    private float hydrationModifier;
    private float reproductiveUrgeModifier = 0.2f;
    public float speedModifier = JoggingSpeed; //100% of maxSpeed in model

    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visibleFoodTargets = new List<GameObject>();
    public List<GameObject> visibleWaterTargets = new List<GameObject>();

    public readonly List<GameObject> heardHostileTargets = new List<GameObject>();
    public readonly List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public readonly List<GameObject> heardPreyTargets = new List<GameObject>();

    public bool IsControllable { get; set; } = false;

    //used for ml, so that it does not spawn a lot of children that might interfere with training
    public bool isInfertile = false;

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
        fsm.Initialize(wanderState);

        animationController = new AnimationController(this);
    }

    protected void Start()
    {
        // Init the NavMesh agent
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;

        //Can be used later.
        baseAngularSpeed = agent.angularSpeed;
        baseAcceleration = agent.acceleration;
        
        agent.speed = animalModel.currentSpeed * Time.timeScale;
        agent.acceleration *= Time.timeScale;
        agent.angularSpeed *= Time.timeScale;


        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();

        SetPhenotype();
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
        switch (state)
        {
            case GoToFood goToFood:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;
                
                //TODO bad practice, hard coded values, this is temporary
                //If food is animal -> Chase!
                if (goToFood != null && goToFood.closestFood != null &&
                    goToFood.closestFood.TryGetComponent<AnimalController>(out _))
                {
                    energyModifier = 0.3f;
                    hydrationModifier = 0.15f;
                    speedModifier = RunningSpeed;
                }
                else
                {
                    speedModifier = JoggingSpeed;
                }

                //Debug.Log("varying parameters depending on state: Eating");
                break;
            case FleeingState _:
                energyModifier = 0.3f;
                hydrationModifier = 0.15f;
                reproductiveUrgeModifier = 1f;
                speedModifier = RunningSpeed;
                //Debug.Log("varying parameters depending on state: FleeingState");
                break;
            case Idle _:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 0.2f;
                //Debug.Log("varying parameters depending on state: Mating");
                break;
            case Waiting _:
                energyModifier = 0f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 1f;
                //Debug.Log("varying parameters depending on state: Mating");
                break;
            case MatingState _:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 0f;


                //Debug.Log("varying parameters depending on state: Wander");
                break;
            case Dead _:
                energyModifier = 0f;
                hydrationModifier = 0f;
                reproductiveUrgeModifier = 0f;
                break;
            default:
                energyModifier = 0.1f;
                hydrationModifier = 0.05f;
                reproductiveUrgeModifier = 0.2f;

                speedModifier = JoggingSpeed;
                //Debug.Log("varying parameters depending on state: Wander");
                break;
        }
        // if (animalModel is WolfModel)
        // {
        //     Debug.Log(speedModifier);
        // }

        animalModel.currentSpeed = animalModel.traits.maxSpeed * speedModifier * animalModel.traits.size;
        agent.speed = animalModel.currentSpeed * Time.timeScale;
    }
    
    public virtual void VaryParameters()
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

        //Debug.Log("Size: " + animalModel.traits.size + " Speed: " + animalModel.currentSpeed);

        animalModel.currentEnergy -= ((animalModel.traits.size * 1) +
                                     (animalModel.traits.size * animalModel.currentSpeed) * energyModifier);
        animalModel.currentHydration -= ((animalModel.traits.size * 1) +
                                        (animalModel.traits.size * animalModel.currentSpeed) * hydrationModifier);
        animalModel.reproductiveUrge += (0.02f * reproductiveUrgeModifier);

        //The age will increase 1 per 1 second.
        animalModel.age += (1);
    }

    protected void EventSubscribe()
    {
        if (tickEventPublisher)
        {
            // every 2 sec
            tickEventPublisher.onParamTickEvent += VaryParameters;
            tickEventPublisher.onParamTickEvent += HandleDeathStatus;
            // every 0.5 sec
            //tickEventPublisher.onSenseTickEvent += fsm.UpdateStatesLogic;    
        }

        fsm.OnStateEnter += ChangeModifiers;

        eatingState.onEatFood += EatFood;

        drinkingState.onDrinkWater += DrinkWater;

        matingState.onMate += Mate;

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
            //tickEventPublisher.onSenseTickEvent -= fsm.UpdateStatesLogic;    
        }


        fsm.OnStateEnter -= ChangeModifiers;

        eatingState.onEatFood -= EatFood;

        drinkingState.onDrinkWater -= DrinkWater;

        matingState.onMate -= Mate;

        animationController.EventUnsubscribe();
    }

    private void Update()
    {
        fsm.UpdateStatesLogic();
    }

    //Set animals size based on traits.
    private void SetPhenotype()
    {
        gameObject.transform.localScale = getNormalizedScale() * animalModel.traits.size;
    }

    public void EatFood(GameObject food, float currentEnergy)
    {

        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal &&
            animalModel.CanEat(edibleAnimal))
        { 
            animalModel.currentEnergy += edibleAnimal.GetEaten();
            Destroy(food);
        }

        if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant && animalModel.CanEat(ediblePlant))
        {
            animalModel.currentEnergy += ediblePlant.GetEaten();
            Destroy(food);
        }
    }

    public void DrinkWater(GameObject water, float currentHydration)
    {
        if (water.gameObject.CompareTag("Water") && !animalModel.HydrationFull)
        {
            animalModel.currentHydration = animalModel.traits.maxHydration;
        }

    }

    //TODO a rabbit should be able to have more than one offspring at a time
    void Mate(GameObject target)
    {
        AnimalController targetAnimalController = null;
        if (target != null)
        { 
            targetAnimalController = target.GetComponent<AnimalController>();

        }


        // make sure target has an AnimalController and that its animalModel is same species
        if (targetAnimalController != null && targetAnimalController.animalModel.IsSameSpecies(animalModel) &&
            targetAnimalController.animalModel.WantingOffspring)
        {
            //Pass energy and hydration to child
            float childEnergy = animalModel.currentEnergy * 0.25f +
                                targetAnimalController.animalModel.currentEnergy * 0.25f;
            float childHydration = animalModel.currentHydration * 0.25f +
                                   targetAnimalController.animalModel.currentHydration * 0.25f;

            //Expend energy and hydration
            if (!isInfertile)
            {
                animalModel.currentEnergy = animalModel.currentEnergy * 0.75f;
                targetAnimalController.animalModel.currentEnergy = targetAnimalController.animalModel.currentEnergy * 0.75f;
                animalModel.currentHydration = animalModel.currentHydration * 0.75f;
                targetAnimalController.animalModel.currentHydration =
                    targetAnimalController.animalModel.currentHydration * 0.75f;

                //Reset both reproductive urges. 
                animalModel.reproductiveUrge = 0f;
                targetAnimalController.animalModel.reproductiveUrge = 0f;     
            }
            

            //TODO promote laborTime to model or something.
            //Go into labor
            // Spawn child as a copy of the father at the position of the mother
            StartCoroutine(GiveBirth(targetAnimalController, childEnergy, childHydration, 5));
        }
    }

    IEnumerator GiveBirth(AnimalController motherController, float newEnergy, float newHydration, float laborTime)
    {
        yield return new WaitForSeconds(laborTime);

        // Generate the offspring traits
        AnimalModel childModel = animalModel.Mate(motherController.animalModel);

        //Don't instantiate bebe if the animal is infertile. Let the check be here since onBirth.invoke is needed in ML-rabbits
        if (!isInfertile)
        {
            GameObject child = Instantiate(gameObject, transform.position, transform.rotation);
        
            child.GetComponent<AnimalController>().animalModel = childModel;
        
            //Set start values
            child.GetComponent<AnimalController>().animalModel.currentEnergy = newEnergy;
            child.GetComponent<AnimalController>().animalModel.currentHydration = newHydration;   
            
            onBirth?.Invoke(this, new OnBirthEventArgs {child = child});
        }
        
    }

    public void DestroyGameObject(float delay)
    {
        Destroy(gameObject, delay);
    }

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */


    //should be refactored so that this logic is in AnimalModel
    private void HandleDeathStatus()
    {
        if (!animalModel.IsAlive)
        {
            //Debug.Log("Energy: "+animalModel.currentEnergy + "Hydration: "+ animalModel.currentHydration);

            // invoke death state with method HandleDeath() in decisionmaker
            actionDeath?.Invoke();
            
            //Stop animal from giving birth once dead.
            StopCoroutine("GiveBirth");

            // unsubscribe all events because we want only want to invoke it once.
            //actionDeath = null;
        }
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        EventUnsubscribe();
    }

    //General method that takes unknown gameobject as input and interacts with the given gameobject depending on what it is. It can be to e.g. consume or to mate
    // It is not guaranteed that the statechange will happen since meetrequirements has to be true for given statechange.
    public void Interact(GameObject target)
    {

        //Dont stop to interact if we are fleeing.
        if (fsm.currentState is FleeingState) return;
        
        //Debug.Log(gameObject.name);
        switch (target.tag)
        {
            case "Water":
                drinkingState.SetTarget(target);
                fsm.ChangeState(drinkingState);
                break;
            case "Plant":
                if (target.TryGetComponent(out PlantController plantController) && animalModel.CanEat(plantController.plantModel))
                {
                    eatingState.SetTarget(target);
                    fsm.ChangeState(eatingState);   
                }
                break;
            case "Animal":
                if (target.TryGetComponent(out AnimalController otherAnimalController))
                {
                    AnimalModel otherAnimalModel = otherAnimalController.animalModel;
                    //if we can eat the other animal we try to do so
                    if (animalModel.CanEat(otherAnimalModel))
                    {
                        eatingState.SetTarget(target);
                        fsm.ChangeState(eatingState);
                    } else if (animalModel.IsSameSpecies(otherAnimalModel))
                    {
                        matingState.SetTarget(target);
                        fsm.ChangeState(matingState);
                    }
                }
                break;
        }

    }
    
    public abstract Vector3 getNormalizedScale();
}