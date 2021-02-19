public class RabbitModel : AnimalModel
{
    public RabbitModel()
    {
        // Set variables specific to wolf
        traits = new Traits(10, 10, 10,10, 10,10,10,10,10,10,10,10);
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }


    public RabbitModel(Traits traits)
    {
        this.traits = traits;
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }
}