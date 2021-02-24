using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnimalModel
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
    
    public int generation { get; set; }
    public Traits traits { get; set; }
    
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                       Parameters                                       */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    public int age { get; set; }
    public float currentHealth { get; set; }
    public float currentEnergy { get; set; }
    public float currentHydration { get; set; }
    public float reproductiveUrge { get; set; }
    
    
    public float currentSpeed { get; set; }

    public bool isEaten = false;

    // decisionMaker subscribes to these actions
    public Action<GameObject> actionPerceivedHostile;

    public AnimalModel(Traits traits, int generation)
    {
        // initializing parameters
        age = 0;
        currentHealth = traits.maxHealth;
        currentEnergy = traits.maxEnergy;
        currentHydration = traits.maxHydration;
        reproductiveUrge = 0.2f;
        this.traits = traits;
    }

    // optional, can be set in the behavior model instead
    // protected string foodType; // herbivore, carnivore, omnivore

    public bool IsAlive()
    {
        return (currentHealth > 0 && currentEnergy > 0 && age < traits.ageLimit && currentHydration > 0) && !isEaten;
    }

    public abstract AnimalModel Mate(AnimalModel otherParent);

    public float GetHealthPercentage()
    {
        return currentHealth / traits.maxEnergy;
    }

    public float GetEnergyPercentage()
    {
        return currentEnergy / traits.maxEnergy;
    }

    public float GetHydrationPercentage()
    {
        return currentHydration / traits.maxHydration;
    }

    public abstract bool CanEat<T>(T obj);

    public abstract bool IsSameSpecies<T>(T obj);

}
