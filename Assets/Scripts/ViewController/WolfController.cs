using UnityEngine;

public class WolfController : AnimalController
{
    protected virtual void Awake()
    {
        animalModel = new WolfModel();
        base.Awake();
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.3f, 0.3f, 0.3f);
    }
    
    public override string GetObjectLabel()
    {
        return "Wolf";
    }
}