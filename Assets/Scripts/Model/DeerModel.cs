using System;
using Model;

public class DeerModel : AnimalModel, IEdible
{

    public DeerModel() : base(new Traits(3.25f, 200, 100, 
                                    300, 4.3f, 10, 
                                    10, 150, 10, 
                                    180, 17, 10), 0)

    {
        // Set variables specific to deer
        nutritionValue = traits.maxEnergy;
    }
    public float nutritionValue { get; set; }

    
    public DeerModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation();
        //TODO logic for determining generation
        return new DeerModel(childTraits, 0);
    }
    
    public override bool CanEat<T>(T obj)
    {
        return obj is PlantModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is DeerModel;
    }
    
    public float GetEaten()
    {
        actionKilled?.Invoke();
        return nutritionValue;
    }
}
