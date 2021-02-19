public class BearModel : AnimalModel
{
    public BearModel()
    {
        // Set variables specific to bear
        traits = new Traits(10, 300, 100, 10,10,10,10,10,10,10,10);
        currentEnergy = 300;
        hydration = 300;
        reproductiveUrge = 0;
    }


    public BearModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 300;
        hydration = 300;
        reproductiveUrge = 0;
    }
    
}
