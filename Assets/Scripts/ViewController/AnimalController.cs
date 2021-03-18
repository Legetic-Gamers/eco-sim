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
    private const float JoggingSpeed = 0.4f;
    private const float RunningSpeed = 1f;

    //Modifiers
    private float energyModifier;
    private float hydrationModifier;
    private float reproductiveUrgeModifier;
    private float speedModifier = JoggingSpeed; //100% of maxSpeed in model

    public readonly List<GameObject> visibleHostileTargets = new List<GameObject>();
    public readonly List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public readonly List<GameObject> visibleFoodTargets = new List<GameObject>();
    public readonly List<GameObject> visibleWaterTargets = new List<GameObject>();

    public readonly List<GameObject> heardHostileTargets = new List<GameObject>();
    public readonly List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public readonly List<GameObject> heardPreyTargets = new List<GameObject>();

    public bool IsControllable { get; set; } = false;
    
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

    public float EatFood(GameObject food)
    {
        float reward = 0f;
        //Access food script to consume the food.
        if (food.GetComponent<AnimalController>()?.animalModel is IEdible edibleAnimal && animalModel.CanEat(edibleAnimal))
        {
            float nutritionReward = edibleAnimal.GetEaten();
            float hunger = animalModel.traits.maxEnergy - animalModel.currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            // normalize reward as a percentage
            reward /= animalModel.traits.maxEnergy;
            animalModel.currentEnergy += nutritionReward;
            Destroy(food);
        }
        
        if (food.GetComponent<PlantController>()?.plantModel is IEdible ediblePlant && animalModel.CanEat(ediblePlant))
        {
            
            float nutritionReward = ediblePlant.GetEaten();
            float hunger = animalModel.traits.maxEnergy - animalModel.currentEnergy;

            //the reward for eating something should be the minimum of the actual nutrition gain and the hunger. Reason is that if an animal eats something that when it is already satisfied it will return a low reward.
            reward = Math.Min(nutritionReward, hunger);
            reward /= animalModel.traits.maxEnergy;
            animalModel.currentEnergy += nutritionReward;
            Destroy(food);
        }

        return reward;
    }

    public float DrinkWater(GameObject water)
    {
        float reward = 0f;
        
        if (water.gameObject.CompareTag("Water") && !animalModel.HydrationFull)
        {
            // the reward should be proportional to how much hydration was gained when drinking
            reward = animalModel.traits.maxHydration - animalModel.currentHydration;
            // normalize reward as a percentage
            reward /= animalModel.traits.maxHydration;
            animalModel.currentHydration = animalModel.traits.maxHydration;
        }

        return reward;
    }

    //TODO a rabbit should be able to have more than one offspring at a time
    void Mate(GameObject target)
    {
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();

        // make sure target has an AnimalController and that its animalModel is same species
        if (targetAnimalController != null && targetAnimalController.animalModel.IsSameSpecies(animalModel))
        {
            // Spawn child as a copy of the father at the position of the mother
            //GameObject child = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
            GameObject child = gameObject;


            // Generate the offspring traits
            AnimalModel childModel = animalModel.Mate(targetAnimalController.animalModel);
            child.GetComponent<AnimalController>().animalModel = childModel;
            //TODO promote laborTime to model or something.
            float childEnergy = animalModel.currentEnergy * 0.25f +
                                targetAnimalController.animalModel.currentEnergy * 0.25f;
            StartCoroutine(GiveBirth(child, childEnergy, 5));

            //Reset both reproductive urges. 
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;

            //Expend energy
            animalModel.currentEnergy = animalModel.currentEnergy * 0.75f;
            targetAnimalController.animalModel.currentEnergy = targetAnimalController.animalModel.currentEnergy * 0.75f;
        }
    }

    IEnumerator GiveBirth(GameObject child, float newEnergy, float laborTime)
    {
        yield return new WaitForSeconds(laborTime);
        //Instantiate here
        child = Instantiate(child, gameObject.transform.position,
            gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
        child.GetComponent<AnimalController>().animalModel.currentEnergy = newEnergy;
        
        onBirth?.Invoke(this,new OnBirthEventArgs{child = child});
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
           
            // unsubscribe all events because we want only want to invoke it once.
            //actionDeath = null;
        }
    }

    public void OnDestroy()
    {
        EventUnsubscribe();
    }

    //General method that takes unknown gameobject as input and interacts with the given gameobject depending on what it is. It can be to e.g. consume or to mate
    // result is the reward for interacting with something
    public float Interact(GameObject gameObject)
    {
        float reward = 0f;

        //Debug.Log(gameObject.name);
        switch (gameObject.tag)
        {
            case "Water":
                return DrinkWater(gameObject);
            case "Plant":
                return EatFood(gameObject);
            case "Animal":
                if (TryGetComponent(out AnimalController otherAnimalController))
                {
                    AnimalModel otherAnimalModel = otherAnimalController.animalModel;
                    //if we can eat the other animal we try to do so
                    if (animalModel.CanEat(otherAnimalModel))
                    {
                        return EatFood(otherAnimalController.gameObject);

                    }

                    if (animalModel.IsSameSpecies(otherAnimalModel))
                    {
                        //Insert code for try to mate and also modify the method so that it returns a float for reward;
                    }
                }
                break;
        }

        return reward;
    }

    public abstract Vector3 getNormalizedScale();

}