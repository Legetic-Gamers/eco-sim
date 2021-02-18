public class DeerModel : AnimalModel
{
    public DeerModel() : base(new Traits(10, 10, 10, 10,10,10,10,10,10,10,10), 0)
    {
        // Deer specific initialization 
        
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
}
