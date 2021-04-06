﻿using System;
using Model;

public class WolfModel : AnimalModel, IEdible
{
    public WolfModel() : base(new Traits(2.35f, 200, 100, 
                                200, 4f, 10, 
                                10, 150, 10, 
                                180, 14, 10), 0)

    {
        nutritionValue = traits.maxEnergy;
    }


    public WolfModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }
    public float nutritionValue { get; set; }



    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation();
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
        actionKilled?.Invoke();
        return nutritionValue;
    }
}
