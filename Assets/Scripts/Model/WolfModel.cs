using Model;

public class WolfModel : AnimalModel, IEdible
{
    public WolfModel() : base(new Traits(3f, 200, 100, 100, 6,10,10,100,10,10,180,10,5),0)
    {
        // Wolf specific initialization 
    }


    public WolfModel(Traits traits, int generation) : base(traits, generation)
    {
        
    }


    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutatation();
        //TODO logic to determine generation
        return new WolfModel(childTraits,0);
    }
    
    
    public override bool CanEat<T>(T obj)
    {
        return obj is RabbitModel || obj is DeerModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is WolfModel;
    }

    public float GetEaten()
    {
        return traits.maxEnergy;
    }
}
