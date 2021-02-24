﻿using Model;

public class DeerModel : AnimalModel
{
    public DeerModel() : base(new Traits(10, 300, 100, 100, 10,10,10,10,10,10,10,10, Traits.BehaviorType.Herbivore, Traits.Species.Rabbit),0)
    {
        // Set variables specific to deer
    }
    
    public DeerModel(Traits traits, int generation) : base(traits, generation)
    {

    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits);
        //TODO logic for determining generation
        return new DeerModel(childTraits, 0);
    }
    
    public override bool CanEat<T>(T obj)
    {
        return obj is PlantModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is DeerModel;
    }
    
}
