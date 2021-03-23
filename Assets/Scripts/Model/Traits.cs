using System;
using System.Reflection;
using UnityEngine;
using Color = UnityEngine.Color;

public class Traits
{
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                         Traits                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    public  float size { get; set; }
    
    public float maxEnergy { get; set; }
    
    public float maxHydration { get; set; }
    
    public float maxHealth { get; set; }

    /// <summary>
    /// based on the article for deciding the velocity of an animal:
    /// more readable: https://www.biorxiv.org/content/10.1101/095018v1.full
    /// alternate version: https://www.uvm.edu/pdodds/research/papers/others/2017/hirt2017a.pdf
    /// some default weights of the animals:
    /// bear avg mass = 180 kg
    /// deer avg mass = 110 kg
    /// wolf avg mass = 70 kg
    /// rabbit avg mass = 5 kg
    ///
    /// default speeds of the animals:
    /// bear avg speed = 56 km/h 
    /// deer avg speed = 48 km/h
    /// wolf avg speed = 55 km/h
    /// rabbit avg speed = 68 km/h (64-72 km/h)
    ///
    ///
    /// function:
    /// Vmax = aM^b * (1 - e^(-hM^i))
    /// a = 26 (acceleration? can change this for each animal)
    /// b = 0.24 (power-law in speed)
    /// e = 2.72 
    /// h = cf = 1
    /// i = d - 1 + g (i = 0.51-1.21 depending on d and g), choose i = 0.6
    /// d = 0.75-0.94 (muscle force)
    /// g = 0.76-1.27 (muscle mass)
    /// c = 1 (otherwise unknown)
    /// f = 1 (otherwise unknown)
    /// M = mass of animal from above
    ///
    /// here are some proposed values of 'a' and 'mass' if using mass instead of size:
    /// bear = 17.5 at mass 180
    /// deer = 20 at mass 110
    /// wolf = 22 at mass 70
    /// rabbit = 46 at mass 5
    /// 
    /// using the proposed values, would get (at default weight) approximately same speed:
    /// bear = 60.9
    /// deer = 61.8
    /// wolf = 61
    /// rabbit = 62.8
    ///
    ///
    /// Note that we are currently using size instead of mass, for simplicity.
    /// </summary>

    public float acceleration { get; set; }

    private float _maxSpeed;
    public float maxSpeed 
    { 
        get => _maxSpeed;

        set
        {
            float bPow = 0.24f;
            float bodymassAccel = acceleration * Mathf.Pow(size, bPow);
            
            float muscleForce = 0.8f;
            float muscleMass = 0.8f;
            float i = muscleForce - 1 + muscleMass;
            float massI = Mathf.Pow(size, i);
            
            float h = 1 * 1; // c * f
            float ePow = 1 - Mathf.Pow(2.71828f, -h * massI);

            float limiter = Mathf.Pow(size, 1f);
            float speed = bodymassAccel * ePow - limiter;
            _maxSpeed = Mathf.Clamp(speed,0.5f,25);
        }
    }
    
    public float maxReproductiveUrge { get; set; }
    
    public float endurance { get; set; }
    
    public float ageLimit { get; set; }
    
    public float temperatureResist { get; set; }
    public float desirability { get; set; }
    public Color furColor = new Color(0.5f, 0.2f, 0.2f, 1.0f); // example

    [Range(0, 360)] public float viewAngle; // affects width of FoV
    public float viewRadius { get; set; } // distance
    
    public float hearingRadius { get; set; }
    

    public Traits(
        float size,
        float maxEnergy, 
        float maxHealth,
        float maxHydration,
        float acceleration,
        float maxReproductiveUrge,
        float endurance, 
        float ageLimit, 
        float temperatureResist, 
        float desirability, 
        float viewAngle, 
        float viewRadius, 
        float hearingRadius)
    {

        this.size = size;
        this.maxEnergy = maxEnergy;
        this.maxHealth = maxHealth;
        this.maxHydration = maxHydration;
        this.acceleration = acceleration;
        maxSpeed = maxSpeed;
        this.maxReproductiveUrge = maxReproductiveUrge;
        this.endurance = endurance;
        this.ageLimit = ageLimit;
        this.temperatureResist = temperatureResist;
        this.desirability = desirability;
        this.viewAngle = viewAngle;
        this.viewRadius = viewRadius;
        this.hearingRadius = hearingRadius;
    }
    
    
    public Traits Crossover(System.Random rng, Traits otherParentTraits, float firstParentAge, float secondParentAge)
    {
        // create a copy of parent one's genes
        Traits childTraits = new Traits(size, maxEnergy, maxHealth, maxHydration, acceleration, maxReproductiveUrge, endurance, ageLimit,
            temperatureResist, desirability, viewAngle, viewRadius, hearingRadius);
        
        try
        {
            // The proposal for using age as a variable to determine crossover ratio between the parents is motivated by; if parent 1 has lived longer than parent 2, it indicates that parent 1 has proven to have a set of genes to live a potentially longer life than parent 2.
            // With other words: parent 2 has not yet proven itself, but parent 1 has, in relativity to each other.
            // This solution will favor the parent that has lived the longest at the time of mating. This means that the older a parent is, the higher chance of passing more of its genes to next generation.
            // Author and proposer: Alexander Huang
            
            // Sum the age to get a total age of the two 
            float totalAge = firstParentAge + secondParentAge;

            // Get a probability of getting the gene from the other parent (which will be used for each trait in the process)
            float threshold = secondParentAge / totalAge;

            // Get type and iterate through all the traits as properties, this solution does not depend on which or how many properties there are
            Type type = GetType();
            foreach (PropertyInfo info in type.GetProperties())
            {
                // randomize a number between 0 and 1
                double rnd = rng.NextDouble();
                
                // if rng value is within the threshold, we set the current trait to the other parents value
                if (rnd < threshold)
                {
                    info.SetValue(childTraits, info.GetValue(otherParentTraits));
                }
            }
        }
        catch (Exception e)
        {
            // One potential exception is if firstParentAge = 0 and secondParentAge = 0, then we will divide by 0 when taking secondParentAge/totalAge
            Debug.Log(e.Message);
        }
        
        return childTraits;
    }

    public void Mutatation(System.Random rng)
    {
        try
        {
            // probability of mutating a trait
            const float mutationRate = 0.05f;
            
            // factor to determine what max value (depending on currentValue) is allowed.
            const float mutationFactor = 2f;
            
            // Get type and iterate through all the traits as properties, this solution does not depend on which or how many properties there is
            Type type = GetType();
            foreach (PropertyInfo info in type.GetProperties())
            {
                // randomize a number between 0 and 1 
                double rnd = rng.NextDouble();

                // if rng value is within the threshold for mutation, we want to mutate the current trait
                if (rnd < mutationRate)
                {
                    float currentValue = (float) info.GetValue(this);
                    float mutatedValue = (float) rng.NextDouble() * currentValue * mutationFactor;
                    info.SetValue(this, mutatedValue);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }
    
    
}
