﻿public class BearModel : AnimalModel
{
    public BearModel()
    {
        // Set variables specific to bear
        traits = new Traits(10, 10, 10, 10,10,10,10,10,10,10,10);
        currentEnergy = 10;
        hydration = 10;
        reproductiveUrge = 0;
    }


}
