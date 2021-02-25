using System;
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

    private TickEventPublisher tickEventPublisher;

    public Action<State> stateChange;
    
    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;
    public Action actionDeath;
    
    [HideInInspector]
    public NavMeshAgent agent;
    
    public FiniteStateMachine Fsm;
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

    float energyModifier = 0;
    float hydrationModifier = 0;
    float reproductiveUrgeModifier = 0;
  

    public bool isControllable { get; set; } = false;

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

    private void changeModifiers(State state)
    {
        if (state is Eating)
        {
            energyModifier = 0f;
            hydrationModifier = 0.05f;
            reproductiveUrgeModifier = 1f;
            //Debug.Log("varying parameters depending on state: Eating");
        } else if (state is FleeingState)
        {
            energyModifier = 1f;
            hydrationModifier = 1f;
            reproductiveUrgeModifier = 1f;
            //Debug.Log("varying parameters depending on state: FleeingState");
        } else if (state is GoToState)
        {
            energyModifier = 0.1f;
            hydrationModifier = 0.05f;
            reproductiveUrgeModifier = 1;
            //Debug.Log("varying parameters depending on state: GoToFood");
        } else if (state is Idle)
        {
            energyModifier = 0f;
            hydrationModifier = 0.05f;
            reproductiveUrgeModifier = 1;
            //Debug.Log("varying parameters depending on state: Mating");
        } else if (state is Mating)
        {
            energyModifier = 0.2f;
            hydrationModifier = 0.2f;
            reproductiveUrgeModifier = 0f;
            //Debug.Log("varying parameters depending on state: Wander");
        } else if (state is Wander)
        {
            energyModifier = 0.1f;
            hydrationModifier = 0.05f;
            reproductiveUrgeModifier = 1f;
            //Debug.Log("varying parameters depending on state: Wander");
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
        /*
        animalModel.currentEnergy--;
        animalModel.currentHydration -= 0.1f; 
        animalModel.reproductiveUrge += 0.1f;
        animalModel.age++;
        */

        animalModel.currentEnergy -= (animalModel.traits.size * 1) + (animalModel.traits.size * animalModel.currentSpeed) * energyModifier;
        animalModel.currentHydration -= (animalModel.traits.size * 1) + (animalModel.traits.size * animalModel.currentSpeed) * hydrationModifier;
        animalModel.reproductiveUrge += 0.1f * reproductiveUrgeModifier;
        //animalModel.age++;
    }

    protected void EventSubscribe()
    {
        tickEventPublisher.onParamTickEvent += VaryParameters;

        Fsm.OnStateEnter += changeModifiers;
        
        eatingState.onEatFood += EatFood;

        drinkingState.onDrinkWater += DrinkWater;

        matingState.onMate += Mate;
        //Debug.Log(gameObject.name + " has subscribed to onParamTickEvent");
    }
    protected void EventUnsubscribe()
    {
        tickEventPublisher.onParamTickEvent -= VaryParameters;

        Fsm.OnStateEnter -= changeModifiers;
        
        eatingState.onEatFood -= EatFood;

        drinkingState.onDrinkWater -= DrinkWater;

        matingState.onMate -= Mate;
        
        animationController.UnSubscribe();
        decisionMaker.EventUnsubscribe();
        
        Debug.Log(gameObject.name + " has unsubscribed from onParamTickEvent.");
    }

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                          Other                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    /*
    public bool IsSameSpecies(AnimalController otherAnimal)
    {
        try
        {
            return otherAnimal.animalModel.traits.species == animalModel.traits.species;
        }
        catch (NullReferenceException)
        {
            return false;
        }
    }
    
    public bool IsPredator =>
        animalModel.traits.behaviorType == Traits.BehaviorType.Carnivore
        || animalModel.traits.behaviorType == Traits.BehaviorType.Omnivore;

    public bool IsPrey => animalModel.traits.behaviorType == Traits.BehaviorType.Herbivore;
    */

    // both hostile and friendly targets, get from FieldOfView and HearingAbility
    //public List<GameObject> heardTargets = new List<GameObject>(); // obsolete
    //public List<GameObject> visibleTargets = new List<GameObject>(); // obsolete
    
    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visibleFoodTargets = new List<GameObject>();
    public List<GameObject> visibleWaterTargets = new List<GameObject>();
    
    public List<GameObject> heardHostileTargets = new List<GameObject>();
    public List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public List<GameObject> heardPreyTargets = new List<GameObject>();
    

    protected void Start()
    {
        // Init the NavMesh agent
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        //Create the FSM.
        Fsm = new FiniteStateMachine();
        animationController = new AnimationController(this);
        
        /*sf = new GoToFood(this, Fsm);
        sw = new GoToWater(this, Fsm);
        sm = new GoToMate(this, Fsm);*/
        eatingState = new Eating(this, Fsm);
        
        
        
        
        fleeingState = new FleeingState(this, Fsm);
        wanderState = new Wander(this, Fsm);
        goToState = new GoToState(this, Fsm);
        idleState = new Idle(this, Fsm);
        drinkingState = new Drinking(this, Fsm);
        matingState = new Mating(this, Fsm);
        deadState = new Dead(this, Fsm);
        Fsm.Initialize(idleState);
        
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe();
        
        decisionMaker = new DecisionMaker(this,animalModel,tickEventPublisher);
    }
    
    
    //should be refactored so that this logic is in AnimalModel
    private void Update()
    {
        if (!animalModel.IsAlive())
        {
            // invoke death state with method HandleDeath() in decisionmaker
            actionDeath?.Invoke();
            // Set state so that it can't change
            Fsm.absorbingState = true;
            // unsubscribe all events because we want only want to invoke it once.
            actionDeath = null;

        }
        Fsm.UpdateStatesLogic();
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

            Debug.Log("MATE");
            animalModel.reproductiveUrge = 0f;
            targetAnimalController.animalModel.reproductiveUrge = 0f;
        }
        
    }
    
    public void DestroyGameObject(float delay)
    {
        Destroy(gameObject, delay);
    }
}
