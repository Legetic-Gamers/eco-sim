using UnityEngine;

public class RabbitController : AnimalController
{
    void Awake()
    {
        animalModel = new RabbitModel();
    }
    public override Vector3 getNormalizedScale()
    {
        return new Vector3(1f, 1f, 1f);
    }
    
}