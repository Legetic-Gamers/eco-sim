using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalModel : MonoBehaviour
{
    /// <summary>
    /// 
    /// Based on Brage's proposal traits document as well as my own (Robin) interpretations, here is, for the sake of ease of access,
    /// a non-exhaustive list of the features and things that can affect the different parameters and traits:
    ///
    /// Parameters:
    /// 
    /// -Energy:
    /// Energy expenditure is affected by various traits of the animal as well as the state of the environment.
    /// Energy expenditure also depends on which state the animal is in.
    /// 
    ///     Always/idle:     
    ///     -Size * (deltaTemp / tempResist) * Const    -Larger the animal, greater the energy expenditure.
    ///     -(Vision + Hearing + Smell) * Const         -The more advanced an animals senses are, the more energy they require
    ///     -currentAge * Const                         -The older an animal, the less energy they will have
    /// 
    ///     High activity state (fleeing, hunting, searching):
    ///     -Size * Speed * Const                       -High activity state will require more movement, therefore more energy spent
    ///
    ///     Mating:
    ///     -OffspringSize / 2 + Const                  -Cannot create energy, so when creating offspring energy has to be lost
    ///
    /// -Health (optional):
    /// Is affected by other animals damage in fights and hunts.*
    /// As decreasing health based on energy and hydration levels is not too realistic/is a bit "gameified",
    /// an alternative to health is to decide the result of hunts/fights based on size and a random factor of luck,
    /// and let low hydration and energy lead directly to death.
    /// 
    ///     -Size * currentEnergy * Const               -Damage depends on size and current energy level, if low then damage is weak
    ///     -if (currentEnergy less than 20% of maxEnergy) do:
    ///         decrementHealth
    ///     -if (hydration less than 20%) do:
    ///         decrementHealth 
    ///
    /// -Hydration:
    /// Is affected by energy consumption, the greater it is, the more hydration is consumed as well.
    ///     -energyConsumption (as a variable) * Const
    ///     -deltaTemp * Const
    ///
    /// -Reproductive Urge:
    /// Only matters when above a certain threshold in regards to energy, hydration, and (optionally) health.
    ///     -Increases when:
    ///         currentEnergy is above 50%
    ///         hydration is above 75%
    ///         health is above 75%
    ///
    /// 
    /// Traits:
    /// What affects them, and/or what they comprise.
    ///
    /// -Size:
    ///     -
    /// -MaxEnergy:
    /// The larger an animal, the greater capacity for energy it has.
    ///     -Const * Size
    /// -MaxHealth:
    /// The larger an animal, the better it is at sustaining damage.
    ///     -Const * Size
    /// -Vision:
    ///     -Reach
    ///     -Width (of FoV)
    /// -Hearing:
    ///     -Reach
    /// -Smell:
    ///     -Reach (as a simplified version of sensitivity)
    /// -MaxAge:
    ///     -
    /// -Speed:
    ///     -Const * Size
    /// -Endurance:
    ///     -Const / Speed
    /// -FurColor:
    ///     -
    /// -Desirability:
    ///     -
    /// -TemperatureResistance:
    ///     -Const * Size   (the constant can be though of as fur and hide thickness)
    ///
    ///
    /// Lastly death occurs when either:
    ///     -Health == 0
    ///     -currentEnergy == 0 (when not going the "decrementHealth if energy less than 20%"-route)                   
    ///     -age == ageMax
    ///     -hydration == 0 (when not going the "decrementHealth if hydration less than 20%"-route)
    /// 
    /// </summary>

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                       Parameters                                       */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    private int age;
    
    protected int currentHealth;
    protected int currentEnergy;
    
    protected int hydration;
    protected int reproductiveUrge;
    
    public bool isAlive = true;
    public bool isControllable = false;

    // optional, can be set in the behavior model instead
    protected string foodType; // herbivore, carnivore, omnivore
    
    // both hostile and friendly targets, get from FieldOfView and HearingAbility
    public List<GameObject> heardTargets = new List<GameObject>();
    public List<GameObject> visibleTargets = new List<GameObject>();
    
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                         Traits                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    public float size;
    public int maxEnergy;
    public int maxHealth; // optional
    
    public float movementSpeed;
    public float endurance;
    
    public int ageLimit;
    
    public float temperatureResist;
    public float desirability;
    public Color furColor = new Color(0.5f, 0.2f, 0.2f, 1.0f); // example
    
    [Range(0, 360)]
    public float viewAngle; // affects width of FoV
    public float viewRadius; // distance
    
    public float hearingRadius;
    
    /* 
     * if we are to have smell:
     * we should probably have some simple "wind" 
     * that determines the direction of the "smell-cone" 
    */
    [Range(0, 360)]
    private float smellingAngle;
    private float smellingRadius;

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
        currentEnergy--; // currentEnergy -= Size * deltaTemp * Const
        Debug.Log("currentEnergy " + currentEnergy + " " + gameObject.name);
    }

    void DecrementHydration()
    {
        hydration--; 
        Debug.Log("thirstLevel " + hydration + " " + gameObject.name);
    }

    void IncrementReproductiveUrge()
    {
        reproductiveUrge++; 
        Debug.Log("reproductiveUrge " + reproductiveUrge + " " + gameObject.name);
    }

    private void IncrementAge()
    {
        age++; 
        Debug.Log(gameObject.name + " has lived for " + age*2 + " seconds.");
        
        if (age > ageLimit) isAlive = false;
    }
    protected void EventSubscribe()
    {
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += DecrementEnergy;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += DecrementHydration;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += IncrementReproductiveUrge;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent += IncrementAge;
    }
    protected void EventUnsubscribe()
    {
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= DecrementEnergy;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= DecrementHydration;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= IncrementReproductiveUrge;
        FindObjectOfType<global::TickEventPublisher>().onParamTickEvent -= IncrementAge;
        
        Debug.Log(gameObject.name + " has unsubscribed from onTickEvent.");
    }

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                          Other                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */
    
    // ugly ugly ugly, but what can you do?
    public void CreateOffspring(GameObject fatherObject, AnimalModel[] parents)
    {
        // Spawn child as a copy of the father at the position of the mother
        GameObject offspringObject = Instantiate(fatherObject, parents[0].transform.position, parents[0].transform.rotation);

        // initialize offspring traits
        AnimalModel offspring = offspringObject.GetComponent<AnimalModel>();

        System.Random rnd = new System.Random();

        var index = rnd.Next(0,2);
        offspring.ageLimit = (int) parents[index].ageLimit;
        index = rnd.Next(0,2);
        offspring.maxEnergy = (int) parents[index].maxEnergy;
        index = rnd.Next(0,2);
        offspring.maxHealth = (int) parents[index].maxHealth;
        index = rnd.Next(0,2);
        offspring.size = parents[index].size;
        index = rnd.Next(0,2);
        offspring.movementSpeed = parents[index].movementSpeed;
        index = rnd.Next(0,2);
        offspring.endurance = parents[index].endurance;
        index = rnd.Next(0,2);
        offspring.temperatureResist = parents[index].temperatureResist;
        index = rnd.Next(0,2);
        offspring.desirability = parents[index].desirability;
        index = rnd.Next(0,2);
        offspring.viewAngle = parents[index].viewAngle;
        index = rnd.Next(0,2);
        offspring.viewRadius = parents[index].viewRadius;
        index = rnd.Next(0,2);
        offspring.hearingRadius =parents[index].hearingRadius;
        index = rnd.Next(0,2);
        offspring.furColor = parents[index].furColor;
    }
}
