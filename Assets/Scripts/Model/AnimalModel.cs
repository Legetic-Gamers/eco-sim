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

    public float age { get; set; }

    //CLAMPING (SETTING A LIMIT) TO ALL LIMITED PARAMETERS.
    private float _currentHealth;

    public float currentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (traits == null)
            {
                _currentHealth = value;
            }
            else
            {
                _currentHealth = Mathf.Clamp(value, 0, traits.maxHealth);
                //Debug.Log("Current: " + _currentHealth + " max: " + traits.maxEnergy);
            }
        }
    }

    private float _currentEnergy;

    public float currentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            if (traits == null)
            {
                _currentEnergy = value;
            }
            else
            {
                _currentEnergy = Mathf.Clamp(value, 0, traits.maxEnergy);
            }
        }
    }

    private float _currentHydration;

    public float currentHydration
    {
        get { return _currentHydration; }
        set
        {
            if (traits == null)
            {
                _currentHydration = value;
            }
            else
            {
                _currentHydration = Mathf.Clamp(value, 0, traits.maxHydration);
            }
        }
    }

    private float _currentSpeed;

    public float currentSpeed
    {
        get { return _currentSpeed; }
        set
        {
            if (traits == null)
            {
                _currentSpeed = value;
            }
            else
            {
                _currentSpeed = Mathf.Clamp(value, 0, traits.maxSpeed);
            }
        }
    }

    //No limit on reproductive urge.
    //public float reproductiveUrge { get; set; }
    
    private float _reproductiveUrge;
    public float reproductiveUrge
    {
        get { return _reproductiveUrge; }
        set
        {
            if (traits == null)
            {
                _reproductiveUrge = value;
            }
            else
            {
                _reproductiveUrge = Mathf.Clamp(value, 0, traits.maxReproductiveUrge);
            }
        }
    }

    public bool IsAlive => (currentHealth > 0 && currentEnergy > 0 && age < traits.ageLimit && currentHydration > 0);

    public float GetHealthPercentage => currentHealth / traits.maxEnergy;

    public float GetEnergyPercentage => currentEnergy / traits.maxEnergy;

    public float GetHydrationPercentage => currentHydration / traits.maxHydration;

    public float GetSpeedPercentage => currentSpeed / traits.maxSpeed;
    
    public float GetUrgePercentage => reproductiveUrge / traits.maxReproductiveUrge;

    public bool EnergyFull => currentEnergy == traits.maxEnergy;

    public bool HighEnergy => currentEnergy / traits.maxEnergy > 0.9f;

    public bool LowEnergy => currentEnergy / traits.maxEnergy < 0.6f;

    public bool HydrationFull => currentHydration == traits.maxHydration;

    public bool HighHydration => currentHydration / traits.maxHydration > 0.9f;

    public bool LowHydration => currentHydration / traits.maxHydration < 0.5f;

    public bool WantingOffspring => reproductiveUrge / traits.maxReproductiveUrge > (traits.maxEnergy - currentEnergy) / traits.maxEnergy && reproductiveUrge / traits.maxReproductiveUrge > (traits.maxHydration - currentHydration) / traits.maxHydration;
    //reproductive urge greater than average of energy and hydration.
    //reproductiveUrge > (currentEnergy + currentHydration) / (traits.maxEnergy + traits.maxHydration);
    // public bool WantingOffspring()
    // {
    //     bool condition =
    //         reproductiveUrge / traits.maxReproductiveUrge > (traits.maxEnergy - currentEnergy) / traits.maxEnergy &&
    //         reproductiveUrge / traits.maxReproductiveUrge >
    //         (traits.maxHydration - currentHydration) / traits.maxHydration;
    //     Debug.Log("Urge: " + reproductiveUrge / traits.maxReproductiveUrge + " Hunger: " + (traits.maxEnergy - currentEnergy)/ traits.maxEnergy + " Thirst: " + (traits.maxHydration - currentHydration)/traits.maxHydration + " Cond: " + condition);
    //    
    //
    //     return condition;
    // }

    public bool LowHealth => currentHealth < 30;

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

    public void UpdateParameters(float energyModifier, float hydrationModifier, float reproductiveUrgeModifier,
        float speedModifier)
    {
        //The age will increase 1 per 1 second.
        age += Time.deltaTime;
        
        currentSpeed = traits.maxSpeed * speedModifier;
        
        currentEnergy -= age + traits.size *
            (traits.viewRadius + traits.hearingRadius + energyModifier * currentSpeed);
        currentHydration -= (traits.size * 1) + (traits.size * currentSpeed) * hydrationModifier;
        reproductiveUrge += 0.1f * reproductiveUrgeModifier;
    }
    
    public abstract AnimalModel Mate(AnimalModel otherParent);
    
    public abstract bool CanEat<T>(T obj);

    public abstract bool IsSameSpecies<T>(T obj);
}