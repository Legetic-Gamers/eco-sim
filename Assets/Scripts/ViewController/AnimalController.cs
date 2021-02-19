using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animal;

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
    void DecrementEnergy() 
    { 
        animal.currentEnergy--; // currentEnergy -= Size * deltaTemp * Const
        //Debug.Log("currentEnergy " + animal.currentEnergy + " " + gameObject.name);
    }

    void DecrementHydration()
    {
        animal.hydration -= 0.1f; 
        Debug.Log("thirstLevel " + animal.hydration + " " + gameObject.name);
    }

    void IncrementReproductiveUrge()
    {
        animal.reproductiveUrge += 0.1f; 
        //Debug.Log("reproductiveUrge " + animal.reproductiveUrge + " " + gameObject.name);
    }

    private void IncrementAge()
    {
        animal.age++; 
        //Debug.Log(gameObject.name + " has lived for " + age*2 + " seconds.");
    }
    protected void EventSubscribe()
    {
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += DecrementEnergy;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += DecrementHydration;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += IncrementReproductiveUrge;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += IncrementAge;
        
        Debug.Log(gameObject.name + " has subscribed to onParamTickEvent");
    }
    protected void EventUnsubscribe()
    {
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= DecrementEnergy;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= DecrementHydration;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= IncrementReproductiveUrge;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= IncrementAge;
        
        Debug.Log(gameObject.name + " has unsubscribed from onParamTickEvent.");
    }

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                          Other                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    public bool IsSameSpecies(AnimalController otherAnimal)
    {
        try
        {
            return otherAnimal.animal.traits.species == animal.traits.species;
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
        AnimalModel childModel = animal.Mate(otherParent.animal);
        // Add coresponding controller
        AnimalController childAnimalController = child.AddComponent<BearController>();
        // Assign traits to child

    }
    
    // both hostile and friendly targets, get from FieldOfView and HearingAbility
    public List<GameObject> heardTargets = new List<GameObject>();
    public List<GameObject> visibleTargets = new List<GameObject>();
    
    protected void Start()
    {
        Debug.Log("Start()");
        // subscribe to the OnTickEvent for parameter handling.
        EventSubscribe();
    }
    
    //should be refactored so that this logic is in AnimalModel
    private void Update()
    {
        if (!animal.IsAlive())
        {
            Debug.Log("Rabbit is ded");
            EventUnsubscribe();
            
            // probably doing this in deathState instead
            Destroy(gameObject, 2.0f);
        }
    }
}
