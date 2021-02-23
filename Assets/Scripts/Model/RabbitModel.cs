public class RabbitModel : AnimalModel
{
    public RabbitModel() : base(new Traits(10, 30, 10, 30, 10,10,10,10,10,10,10,10, Traits.BehaviorType.Herbivore, Traits.Species.Rabbit),0)
    {
        // Rabbit specific initialization 
        
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        this.traits = traits;
        currentEnergy = 20;
        hydration = 30;
        reproductiveUrge = 40;
        Traits childTraits = traits.Crossover(otherParent.traits);
        //TODO logic for determining generation
        return new RabbitModel(childTraits, 0);
    }
}