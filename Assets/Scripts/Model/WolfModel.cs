public class WolfModel : AnimalModel
{
    public WolfModel() : base(new Traits(10, 300, 100, 10,10,10,10,10,10,10,10, Traits.BehaviorType.Carnivore, Traits.Species.Wolf),0)
    {
        // Wolf specific initialization 
    }


    public WolfModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }


    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits);
        //TODO logic to determine generation
        return new WolfModel(childTraits,0);
    }
}
