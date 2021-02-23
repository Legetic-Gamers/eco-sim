public class RabbitModel : AnimalModel
{
    public RabbitModel() : base(new Traits(10, 100, 100, 100, 10,100,100,100,100,100,100,100, Traits.BehaviorType.Herbivore, Traits.Species.Rabbit),0)
    {
        // Rabbit specific initialization 
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