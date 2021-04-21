using UnityEngine;

public class WolfController : AnimalController
{
    new void Awake()
    {
        base.Awake();
        animalModel = new WolfModel();
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