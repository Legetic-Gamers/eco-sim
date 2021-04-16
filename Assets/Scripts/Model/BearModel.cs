using System;
using Model;

public class BearModel : AnimalModel
{

    public BearModel() : base(new Traits(5f, 300, 100, 
                                400, 7f, 15, 
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
        childTraits.Mutation(0.05f);
        
        return new BearModel(childTraits, Math.Max(generation, otherParent.generation) + 1);
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
