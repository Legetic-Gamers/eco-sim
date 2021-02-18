public class RabbitModel : AnimalModel
{
    public RabbitModel() : base(new Traits(10, 10, 10, 10,10,10,10,10,10,10,10), 0)
    {
        // Deer specific initialization 
        
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits);
        //TODO logic for determining generation
        return new RabbitModel(childTraits, 0);
    }
}