using UnityEngine;

public class RabbitController : AnimalController
{
    new void Awake()
    {
        base.Awake();
        animalModel = new RabbitModel();
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