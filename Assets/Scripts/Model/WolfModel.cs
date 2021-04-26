using System;
using Model;

public class WolfModel : AnimalModel, IEdible
{
    public WolfModel() : base(new Traits(2.35f, 250, 100, 
                                200, 6f, 10, 
                                10, 90, 10, 
                                180, 14, 10), 0)

    {
        nutritionValue = traits.maxEnergy;
        gestationTime = 15; // 10 weeks IRL
    }


    public WolfModel(Traits traits, int generation) : base(traits, generation)
    {
        nutritionValue = traits.maxEnergy;
        gestationTime = 15;
    }
    public float nutritionValue { get; set; }



    public override AnimalModel Mate(AnimalModel otherParent)
    {
        Traits childTraits = traits.Crossover(otherParent.traits, age, otherParent.age);
        childTraits.Mutation(0.05f);
        
        return new WolfModel(childTraits,Math.Max(generation, otherParent.generation) + 1);
    }
    
    
    public override bool CanEat<T>(T obj)
    {
        //Debug.Log(obj.GetType().Name);
        return obj is RabbitModel || obj is DeerModel;
    }
    
    public override bool IsSameSpecies<T>(T obj)
    {
        return obj is WolfModel;
    }


    public float GetEaten()
    {
        actionKilled?.Invoke();
        return nutritionValue;
    }
}
