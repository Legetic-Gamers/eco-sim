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
    public float maxHealth { get; set; } // optional
    
    public float maxSpeed { get; set; }
    public float endurance { get; set; }
    
    public float ageLimit { get; set; }
    
    public float temperatureResist { get; set; }
    public float desirability { get; set; }
    public Color furColor = new Color(0.5f, 0.2f, 0.2f, 1.0f); // example

    [Range(0, 360)] public float viewAngle;// affects width of FoV
    public float viewRadius { get; set; } // distance
    
    public float hearingRadius { get; set; }
    
    
    /* 
     * if we are to have smell:
     * we should probably have some simple "wind" 
     * that determines the direction of the "smell-cone" 

    [Range(0, 360)] private float smellingAngle;
    private float smellingRadius { get; set; }
    */

    public Traits(
        float size,
        float maxEnergy, 
        float maxHealth,
        float maxHydration,
        float maxSpeed,
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
        this.maxSpeed = maxSpeed;
        this.endurance = endurance;
        this.ageLimit = ageLimit;
        this.temperatureResist = temperatureResist;
        this.desirability = desirability;
        this.viewAngle = viewAngle;
        this.viewRadius = viewRadius;
        this.hearingRadius = hearingRadius;
    }
    
    
    public Traits Crossover(Traits otherParentTraits, float firstParentAge, float secondParentAge)
    {
        // create a copy of parent one's genes
        Traits childTraits = new Traits(size, maxEnergy, maxHealth, maxHydration, maxSpeed, endurance, ageLimit,
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
            
            System.Random rnd = new System.Random();

            // Get type and iterate through all the traits as properties, this solution does not depend on which or how many properties there is
            Type type = GetType();
            foreach (PropertyInfo info in type.GetProperties())
            {
                // randomize a number between 0 and 1 
                double rng = rnd.NextDouble();
                
                // if rng value is within the threshold, we set the current trait to the other parents value
                if (rng < threshold)
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

    public void Mutatation()
    {
        try
        {
            // probability of mutating a trait
            const float mutationRate = 0.05f;
            
            // factor to determine what max value (depending on currentValue) is allowed.
            const float mutationFactor = 2f;

            System.Random rnd = new System.Random();

            // Get type and iterate through all the traits as properties, this solution does not depend on which or how many properties there is
            Type type = GetType();
            foreach (PropertyInfo info in type.GetProperties())
            {
                // randomize a number between 0 and 1 
                double rng = rnd.NextDouble();

                // if rng value is within the threshold for mutation, we want to mutate the current trait
                if (rng < mutationRate)
                {
                    float currentValue = (float) info.GetValue(this);
                    float mutatedValue = (float) rnd.NextDouble() * currentValue * mutationFactor;
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
