using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

public class WolfModel : AnimalModel, IEdible
{
    
    public float nutritionValue { get; set; }


    public WolfModel() : base(new Traits(3f, 100, 100, 100, 2,1,10,100,10,10,180,15,7),0)
    {
        nutritionValue = traits.maxEnergy;
    }


    public WolfModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }


    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutatation();
        //TODO logic to determine generation
        return new WolfModel(childTraits,0);
    }
    
    
    public override bool CanEat<T>(T obj)
    {
        //Debug.Log(obj.GetType().Name);
        return obj is RabbitModel || obj is DeerModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is WolfModel;
    }


    public float GetEaten()
    {
        return nutritionValue;
    }
}
