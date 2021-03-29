using System;
using Model;

public class DeerModel : AnimalModel, IEdible
{
    public DeerModel() : base(new Traits(3.25f, 100, 100, 
                                    100, 5.6f, 10, 
                                    10, 10, 10, 
                                    180, 10, 10), 0)
    {
        // Set variables specific to deer
    }
    
    public DeerModel(Traits traits, int generation) : base(traits, generation)
    {

    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation();
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

    public float GetEaten()
    {
        return 10f;
    }
}
