using UnityEngine;

public class DeerController : AnimalController
{
    new void Awake()
    {
        base.Awake();
        animalModel = new DeerModel();
    }
    
    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.30f, 0.30f, 0.30f);
    }

    
}