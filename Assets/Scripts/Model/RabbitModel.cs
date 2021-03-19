using System;
using Model;

public class RabbitModel : AnimalModel,IEdible
{
    public float nutritionValue { get; set; }

    public RabbitModel() : base(new Traits(1f, 40, 100, 50, 3,1,10,60,10,10,120,10,5),0)
    {
        // Rabbit specific initialization 
        
        //nutrionValue is same as maxEnergy in this case
        nutritionValue = traits.maxEnergy;
    }
    
    public RabbitModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutatation();
        //TODO logic for determining generation
        return new RabbitModel(childTraits, 0);
    }

    public float GetEaten()
    {
        return nutritionValue;
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