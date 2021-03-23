using System;
public class BearModel : AnimalModel
{
    public BearModel() : base(new Traits(5f, 100, 100, 100, 6f, 10,10,10,10,10,180,10,10),0)
    {
        // Set variables specific to bear
    }

    public BearModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(Random rng, AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(rng, otherParent.traits, age, otherParent.age);
        childTraits.Mutatation(rng);
        //TODO logic to determine generation
        return new BearModel(childTraits, 0);
    }
    
    public override bool CanEat<T>(T obj)
    {
        return obj is WolfModel || obj is RabbitModel || obj is DeerModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is BearModel;
    }
}
