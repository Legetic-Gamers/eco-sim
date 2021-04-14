using System;
using Model;
using UnityEngine;
using Random = System.Random;

public class RabbitModel : AnimalModel,IEdible
{
    public float nutritionValue { get; set; }

    public RabbitModel() : base(new Traits(1f, 100, 100, 
                                    50, 6.65f, 5f, 
                                    10,40, 10, 
                                    160, 13, 7), 0)

    {
        // Rabbit specific initialization 
        
        //nutrionValue is same as maxEnergy in this case
        nutritionValue = traits.maxEnergy;
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation();
        
        return new RabbitModel(childTraits, Math.Max(generation, otherParent.generation) + 1);
    }

    public float GetEaten()
    {
        actionKilled?.Invoke();
        return nutritionValue;
    }

    public override bool CanEat<T>(T obj)
    {
        return obj is PlantModel;
    }

    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is RabbitModel;
    }
}