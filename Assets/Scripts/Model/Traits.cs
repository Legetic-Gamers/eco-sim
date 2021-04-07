using System;
using System.Reflection;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = System.Random;

public class Traits
{
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                         Traits                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private float _size;
    public float size
    {
        get => _size;
        set => _size = Mathf.Clamp(value, 0.1f, 20); 
    }
    
    public float maxEnergy { get; set; }
    
    public float maxHydration { get; set; }
    
    public float maxHealth { get; set; }

    /// <summary>
    /// based on this article for deciding the velocity of an animal:
    /// 1: https://www.biorxiv.org/content/10.1101/095018v1.full
    /// 2: https://www.uvm.edu/pdodds/research/papers/others/2017/hirt2017a.pdf
    /// 
    /// function:
    /// Vmax = aM^b * (1 - e^(-hM^i))
    /// 
    /// Below are the suggested values from the article.
    /// However, we are using size instead of mass, 
    /// and the Vmax does not translate to the expected speed in Unity.
    /// Therefore we will have to use some tweaked values to get the
    /// speeds that we want, relative to the sizes that we use.
    /// 
    /// </summary>
    /// <param name="a"> The acceleration of the animal. </param>
    /// <param name="M"> Mass of animal, but we use size instead. </param>
    /// <param name="b">Power-law in speed. According to article should be 0.24. </param>
    /// <param name="e"> Euler's number. </param>
    /// <param name="h = c * f"> Some constants. </param>
    /// <param name="d"> 0.75-0.94, Muscle Force. </param>
    /// <param name="g"> 0.76-1.27, Muscle Mass. </param>
    /// <param name="i"> 0.51-1.21, i = d - 1 + g, according to article should be 0.6. </param>

    private float _acceleration;
    public float acceleration
    {
        get => _acceleration;
        set => _acceleration = Mathf.Clamp(value, 3.5f, 15);
    }

    private float _maxSpeed;
    public float maxSpeed 
    { 
        get => _maxSpeed;

        set
        {
            float bPow = 0.4f;
            float bodymassAccel = acceleration * Mathf.Pow(size, bPow);
            
            float muscleForce = 0.94f;
            float muscleMass = 1.4f;
            float i = muscleForce - 1 + muscleMass;
            float massI = Mathf.Pow(size, i);
            
            float c = 1.5f; // h = c * f
            float ePow = 1 - Mathf.Pow(2.71828f, -c * massI);

            float sizeLimiter = Mathf.Pow(size, i);
            float speed = bodymassAccel * ePow - sizeLimiter;
            _maxSpeed = Mathf.Clamp(speed,0.4f,25); // actual limit is ~15
        }
    }

    private float _maxReproductiveUrge;
    public float maxReproductiveUrge
    {
        get => _maxReproductiveUrge; 
        set => _maxReproductiveUrge = Mathf.Clamp(value, 1,40);
    }
    
    public float endurance { get; set; }

    private float _ageLimit;
    public float ageLimit
    {
        get => _ageLimit; 
        set => _ageLimit = Mathf.Clamp(value, 1, 2000);
    }
    
    public float desirability { get; set; }
    public Color furColor = new Color(0.5f, 0.2f, 0.2f, 1.0f); // example

    [Range(0, 360)] public float viewAngle; // affects width of FoV
    private float _viewRadius; // distance
    public float viewRadius
    {
        get => _viewRadius; 
        set => _viewRadius = Mathf.Clamp(value, 0, 200);
    }

    private float _hearingRadius;
    public float hearingRadius
    {
        get => _hearingRadius; 
        set => _hearingRadius = Mathf.Clamp(value, 0, 200);
    }
    

    public Traits(
        float size,
        float maxEnergy, 
        float maxHealth,
        float maxHydration,
        float acceleration,
        float maxReproductiveUrge,
        float endurance, 
        float ageLimit, 
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
        this.desirability = desirability;
        this.viewAngle = viewAngle;
        this.viewRadius = viewRadius;
        this.hearingRadius = hearingRadius;
    }
    
    
    public Traits Crossover( Traits otherParentTraits, float firstParentAge, float secondParentAge)
    {
        Random rng = new Random();
        
        // create a copy of parent one's genes
        Traits childTraits = new Traits(size, maxEnergy, maxHealth, maxHydration, acceleration, maxReproductiveUrge, endurance, ageLimit,
             desirability, viewAngle, viewRadius, hearingRadius);
        
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

    public void Mutation()
    {
        try
        {
            Random rng = new Random();
            // probability of mutating a trait
            const float mutationRate = 0.95f;
            
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
