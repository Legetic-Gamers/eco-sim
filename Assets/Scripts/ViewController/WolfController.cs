public class WolfController : AnimalController
{
    void Awake()
    {
        base.Awake();
        animalModel = new WolfModel();
    }

    
}