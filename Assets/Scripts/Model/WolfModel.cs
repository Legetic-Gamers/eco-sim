public class WolfModel : AnimalModel
{
    public WolfModel()
    {
        // Set variables specific to wolf
        traits = new Traits(10, 10, 10, 10,10,10,10,10,10,10,10);
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }
    public WolfModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }


}
