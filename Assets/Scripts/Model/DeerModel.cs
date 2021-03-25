using Model;

public class DeerModel : AnimalModel, IEdible
{
    public float nutritionValue { get; set; }
    
    public DeerModel() : base(new Traits(3, 100, 100, 100, 2.2f,1,10,200,10,10,180,10,10),0)
    {
        // Set variables specific to deer
        nutritionValue = traits.maxEnergy;
    }
    
    public DeerModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
    }

    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
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
        return nutritionValue;
    }
}
