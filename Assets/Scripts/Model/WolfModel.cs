using System;
using Model;

public class WolfModel : AnimalModel, IEdible
{
    public WolfModel() : base(new Traits(2.35f, 250, 100, 
                                200, 6.25f, 75f, 
                                10, 75, 10, 
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
    public bool isEaten { get; set; }



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
        //Only return nutrition if wolf has not already been eaten.
        if (!isEaten)
        {
            actionKilled?.Invoke();
            isEaten = true;
            return nutritionValue;
        }

        return 0;
    }
}
