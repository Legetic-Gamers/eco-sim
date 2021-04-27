using System;
using Model;
using UnityEngine;
using Random = System.Random;

public class RabbitModel : AnimalModel,IEdible
{
    public float nutritionValue { get; set; }

    public RabbitModel() : base(new Traits(1f, 100, 50, 
                                    100, 6f, 20f, 
                                    10,30, 10, 
                                    160, 13, 7), 0)

    {
        // Rabbit specific initialization 
        
        //nutritionValue is same as maxEnergy in this case
        nutritionValue = traits.maxEnergy;
        offspringCount = 3; // rabbits will get more offspring
        gestationTime = 10; // 4 weeks IRL 
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy / 2;
        offspringCount = 2;
        gestationTime = 10;
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation(0.05f);
        
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