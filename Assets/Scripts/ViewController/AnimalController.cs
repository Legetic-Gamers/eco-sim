using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using UnityEngine;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animalModel;

    private Animal animal;

    private TickEventPublisher tickEventPublisher;

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
    void DecrementEnergy() 
    { 
        animalModel.currentEnergy--; // currentEnergy -= Size * deltaTemp * Const
        //Debug.Log("currentEnergy " + animal.currentEnergy + " " + gameObject.name);
    }

    void DecrementHydration()
    {
        animalModel.hydration--;
        Debug.Log(animalModel.hydration);
        //Debug.Log("thirstLevel " + animal.hydration + " " + gameObject.name);
    }

    void IncrementReproductiveUrge()
    {
        animalModel.reproductiveUrge++; 
        //Debug.Log("reproductiveUrge " + animal.reproductiveUrge + " " + gameObject.name);
    }

    private void IncrementAge()
    {
        animalModel.age++; 
        //Debug.Log(gameObject.name + " has lived for " + age*2 + " seconds.");
        
        if (animalModel.age > animalModel.traits.ageLimit) animalModel.isAlive = false;
    }
    protected void EventSubscribe(TickEventPublisher eventPublisher)
    {
        eventPublisher.onParamTickEvent += DecrementEnergy;
        eventPublisher.onParamTickEvent += DecrementHydration;
        eventPublisher.onParamTickEvent += IncrementReproductiveUrge;
        eventPublisher.onParamTickEvent += IncrementAge;
        
        Debug.Log(gameObject.name + " has subscribed to onParamTickEvent");
    }
    protected void EventUnsubscribe(TickEventPublisher eventPublisher)
    {
        eventPublisher.onParamTickEvent -= DecrementEnergy;
        eventPublisher.onParamTickEvent -= DecrementHydration;
        eventPublisher.onParamTickEvent -= IncrementReproductiveUrge;
        eventPublisher.onParamTickEvent -= IncrementAge;
        
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
        Traits childTraits = animalModel.traits.Crossover(otherParent.animalModel.traits);
        // Add coresponding controller
        AnimalController childAnimalController = child.AddComponent<BearController>();
        // Assign traits to child
        childAnimalController.animalModel.traits = childTraits;

    }
    
    // both hostile and friendly targets, get from FieldOfView and HearingAbility
    public List<GameObject> heardTargets = new List<GameObject>();
    public List<GameObject> visibleTargets = new List<GameObject>();
    
    protected void Start()
    {
        Debug.Log("Start()");
        // subscribe to the OnTickEvent for parameter handling.
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        EventSubscribe(tickEventPublisher);

        animal = GetComponent<Animal>();

        DecisionMaker decisionMaker = new DecisionMaker(animal,this,animalModel,tickEventPublisher);
    }
    
    //should be refactored so that this logic is in AnimalModel
    private void Update()
    {
        if (animalModel.isAlive && animalModel.currentEnergy <= 0 && animalModel.hydration <= 0)
        {
            animalModel.isAlive = false; 
            EventUnsubscribe(tickEventPublisher);
            
            // probably doing this in deathState instead
            Destroy(gameObject, 2.0f);
        }
    }
}
