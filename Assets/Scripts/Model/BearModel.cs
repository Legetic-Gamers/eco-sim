﻿using System;
using Model;

public class BearModel : AnimalModel
{

    public BearModel() : base(new Traits(5f, 500, 100, 
                                400, 5f, 15, 
                                10, 400, 10, 
                                180, 12, 10), 0)

    {
        // Set variables specific to bear
    }

    public BearModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation();
        //TODO logic to determine generation
        return new BearModel(childTraits, 0);
    }
    
    public override bool CanEat<T>(T obj)
    {
        return obj is WolfModel || obj is RabbitModel || obj is DeerModel || obj is PlantModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is BearModel;
    }
}
