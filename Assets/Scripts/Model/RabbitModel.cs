using System;
using Model;
using UnityEngine;

public class RabbitModel : AnimalModel,IEdible
{

    public RabbitModel() : base(new Traits(1f, 20, 100, 30, 3,1,10,40,10,10,120,10,3),0)
    {
        // Rabbit specific initialization 
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutatation();
        return new RabbitModel(childTraits, Mathf.Max(otherParent.generation, this.generation) + 1);
    }

    public float GetEaten()
    {
        return traits.maxEnergy;
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