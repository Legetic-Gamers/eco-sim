using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalController : MonoBehaviour
{
    public AnimalModel animal;

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
        Debug.Log("currentEnergy " + animal.currentEnergy + " " + gameObject.name);
    }

    void DecrementHydration()
    {
        animal.hydration--; 
        Debug.Log("thirstLevel " + animal.hydration + " " + gameObject.name);
    }

    void IncrementReproductiveUrge()
    {
        animal.reproductiveUrge++; 
        Debug.Log("reproductiveUrge " + animal.reproductiveUrge + " " + gameObject.name);
    }

    private void IncrementAge()
    {
        animal.age++; 
        //Debug.Log(gameObject.name + " has lived for " + age*2 + " seconds.");
        
        if (animal.age > animal.ageLimit) animal.isAlive = false;
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
    
    // ugly ugly ugly, but what can you do?
    // maybe offspring logic should be in the model?
    public void CreateOffspring(GameObject fatherObject, AnimalController[] parents)
    {
        // Spawn child as a copy of the father at the position of the mother
        GameObject offspringObject = Instantiate(fatherObject, parents[0].transform.position, parents[0].transform.rotation);

        // initialize offspring traits
        AnimalController offspring = offspringObject.GetComponent<AnimalController>();

        System.Random rnd = new System.Random();

        var index = rnd.Next(0,2);
        offspring.animal.ageLimit = (int) parents[index].animal.ageLimit;
        index = rnd.Next(0,2);
        offspring.animal.maxEnergy = (int) parents[index].animal.maxEnergy;
        index = rnd.Next(0,2);
        offspring.animal.maxHealth = (int) parents[index].animal.maxHealth;
        index = rnd.Next(0,2);
        offspring.animal.size = parents[index].animal.size;
        index = rnd.Next(0,2);
        offspring.animal.movementSpeed = parents[index].animal.movementSpeed;
        index = rnd.Next(0,2);
        offspring.animal.endurance = parents[index].animal.endurance;
        index = rnd.Next(0,2);
        offspring.animal.temperatureResist = parents[index].animal.temperatureResist;
        index = rnd.Next(0,2);
        offspring.animal.desirability = parents[index].animal.desirability;
        index = rnd.Next(0,2);
        offspring.animal.viewAngle = parents[index].animal.viewAngle;
        index = rnd.Next(0,2);
        offspring.animal.viewRadius = parents[index].animal.viewRadius;
        index = rnd.Next(0,2);
        offspring.animal.hearingRadius =parents[index].animal.hearingRadius;
        index = rnd.Next(0,2);
        offspring.animal.furColor = parents[index].animal.furColor;
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
        if (animal.isAlive && animal.currentEnergy <= 0 && animal.hydration <= 0)
        {
            animal.isAlive = false; 
            EventUnsubscribe();
            
            // probably doing this in deathState instead
            Destroy(gameObject, 2.0f);
        }
    }
}
