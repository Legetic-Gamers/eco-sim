public class BearModel : AnimalModel
{
    public BearModel()
    {
        // Set variables specific to bear
        traits = new Traits(10, 300, 100,300, 10,10,10,10,10,10,10,3);
        currentEnergy = 300;
        hydration = 300;
        reproductiveUrge = 400;
        traits.behaviorType = Traits.BehaviorType.Herbivore;
    }


    public BearModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 300;
        hydration = 300;
        reproductiveUrge = 0;
    }
    
}
