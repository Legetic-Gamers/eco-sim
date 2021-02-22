public class BearModel : AnimalModel
{
    public BearModel()
    {
        // Set variables specific to bear
        traits = new Traits(10, 30, 100,30, 10,10,10,10,10,60,10,5);
        currentEnergy = 20;
        hydration = 30;
        reproductiveUrge = 40;
        traits.behaviorType = Traits.BehaviorType.Herbivore;
    }


    public BearModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 20;
        hydration = 30;
        reproductiveUrge = 40;
    }
    
}
