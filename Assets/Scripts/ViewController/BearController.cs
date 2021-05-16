using UnityEngine;

public class BearController : AnimalController
{
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        animalModel = new BearModel();
        base.Awake();
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
