public class BearModel : AnimalModel
{
    public BearModel() : base(new Traits(1, 300, 100, 100, 5,10,10,10,10,10,10,10, Traits.BehaviorType.Herbivore, Traits.Species.Deer),0)
    {
        // Set variables specific to bear
    }

    public BearModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits);
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
