public class RabbitController : AnimalController
{
    void Awake()
    {
        base.Awake();
        animalModel = new RabbitModel();
    }

    
}