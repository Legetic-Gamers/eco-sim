using UnityEngine;

public class RabbitController : AnimalController
{
    new void Awake()
    {
        animalModel = new RabbitModel();
        base.Awake();
    }
    public override Vector3 getNormalizedScale()
    {
        return new Vector3(1f, 1f, 1f);
    }
    
    public override string GetObjectLabel()
    {
        return "Rabbit";
    }
    
}