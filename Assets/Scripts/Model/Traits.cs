using System;
using UnityEngine;
using Color = UnityEngine.Color;

public class Traits
{
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */
    /*                                         Traits                                         */
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    public enum BehaviorType
    {
        Carnivore,
        Herbivore,
        Omnivore
    }

    public enum Species // expand
    {
        Bear,
        Wolf,
        Rabbit,
        Deer
    }

    public BehaviorType behaviorType { get; set; }

    public Species species { get; set; }
    
    public bool IsPredator
    {
        get{
            try
            {
                return behaviorType == BehaviorType.Carnivore
                       || behaviorType == BehaviorType.Omnivore;

            }
            catch (NullReferenceException)
            {
                return false;
            }   
        }
        
    }
    public bool IsPrey
    {
        get{
            try
            {
                return behaviorType == BehaviorType.Herbivore;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }

    public  float size { get; set; }
    public int maxEnergy { get; set; }
    
    public int maxHydration { get; set; }
    public int maxHealth { get; set; } // optional
    
    public float maxSpeed { get; set; }
    public float endurance { get; set; }
    
    public int ageLimit { get; set; }
    
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
        int maxEnergy, 
        int maxHealth,
        int maxHydration,
        float maxSpeed,
        float endurance, 
        int ageLimit, 
        float temperatureResist, 
        float desirability, 
        float viewAngle, 
        float viewRadius, 
        float hearingRadius,
        BehaviorType behaviorType,
        Species species)
    {

        this.size = size;

        //this.behaviorType = behaviorType;
        //this.species = species;

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
        this.behaviorType = behaviorType;
        this.species = species;
    }
    
    
    public Traits Crossover(Traits otherParent)
    {
        Traits childTraits = new Traits(10,10,10,100, 10,10,10,10,10,10,10,10, otherParent.behaviorType, otherParent.species);
        //TODO crossover logic to generate a gene between trait of other parent and this.
        return childTraits;
        
        /*
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
        */

    }
    
    
}
