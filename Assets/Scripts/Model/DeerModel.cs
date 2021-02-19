public class DeerModel : AnimalModel
{
    public DeerModel()
    {
        // Set variables specific to deer
        traits = new Traits(10, 10, 10,10, 10,10,10,10,10,10,10,10);
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }

    public DeerModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }
}
