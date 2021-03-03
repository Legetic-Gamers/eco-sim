public class DeerController : AnimalController
{
    void Awake()
    {
        base.Awake();
        animalModel = new DeerModel();
    }

    
}