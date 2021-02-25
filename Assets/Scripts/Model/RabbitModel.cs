using System;
using Model;

public class RabbitModel : AnimalModel,IEdible
{

    public RabbitModel() : base(new Traits(1f, 100, 100, 100, 3,10,100,10,10,180,10,10, Traits.BehaviorType.Herbivore, Traits.Species.Rabbit),0)
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

    public float GetEaten()
    {
        isEaten = true;
        return 100f;
    }

    public override bool CanEat<T>(T obj)
    {
        return obj is PlantModel;
    }

    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is RabbitModel;
    }
}