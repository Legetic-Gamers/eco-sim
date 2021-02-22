using System;
using System.Collections.Generic;
using AnimalsV2;
using UnityEngine;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animalModel;

    private Animal animal;

    private TickEventPublisher tickEventPublisher;

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
        animalModel.currentEnergy--;
        animalModel.hydration -= 0.1f; 
        animalModel.reproductiveUrge += 0.1f;
        animalModel.age++; 
    }

    protected void EventSubscribe()
    {
        tickEventPublisher.onParamTickEvent += VaryParameters;
        
        Debug.Log(gameObject.name + " has subscribed to onParamTickEvent");
    }
    protected void EventUnsubscribe()
    {
        tickEventPublisher.onParamTickEvent -= VaryParameters;
        
        Debug.Log(gameObject.name + " has unsubscribed from onParamTickEvent.");
    }

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                          Other                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
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

    //TODO a rabbit should be able to have more than one offspring at a time
    void CreateOffspring(AnimalController otherParent)
    {

        // Spawn child as a copy of the father at the position of the mother
        GameObject child = Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation); //NOTE CHANGE SO THAT PREFAB IS USED
        // Generate the offspring traits
        AnimalModel childModel = animalModel.Mate(otherParent.animalModel);
        // Add coresponding controller
        AnimalController childAnimalController = child.AddComponent<BearController>();
        // Assign traits to child
    }
    
    // both hostile and friendly targets, get from FieldOfView and HearingAbility
    //public List<GameObject> heardTargets = new List<GameObject>(); // obsolete
    //public List<GameObject> visibleTargets = new List<GameObject>(); // obsolete
    
    public List<GameObject> visibleHostileTargets = new List<GameObject>();
    public List<GameObject> visibleFriendlyTargets = new List<GameObject>();
    public List<GameObject> visiblePreyTargets = new List<GameObject>();
    
    public List<GameObject> heardHostileTargets = new List<GameObject>();
    public List<GameObject> heardFriendlyTargets = new List<GameObject>();
    public List<GameObject> heardPreyTargets = new List<GameObject>();
    
    protected void Start()
    {
        Debug.Log("Start()");
        // subscribe to the OnTickEvent for parameter handling.
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        EventSubscribe();

        animal = GetComponent<Animal>();

        DecisionMaker decisionMaker = new DecisionMaker(animal,this,animalModel,tickEventPublisher);
    }
    
    
    //should be refactored so that this logic is in AnimalModel
    private void Update()
    {
        if (!animalModel.IsAlive())
        {
            Debug.Log("Rabbit is ded");
            EventUnsubscribe();
            
            // probably doing this in deathState instead
            Destroy(gameObject, 2.0f);
        }
    }
}
