using UnityEngine;

public class BearController : AnimalController
{
    // Start is called before the first frame update
    new void Awake()
    {
        base.Awake();
        animalModel = new BearModel();
    }

    public override string GetObjectLabel()
    {
        return "Bear";
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.2f, 0.2f, 0.2f);
    }
    
    
}
