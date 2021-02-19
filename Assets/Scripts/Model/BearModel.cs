public class BearModel : AnimalModel
{
    public BearModel() : base(new Traits(10, 300, 100, 10,10,10,10,10,10,10,10, Traits.BehaviorType.Omnivore, Traits.Species.Bear),0)
    {

        // Bear specific initialization 
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
}
