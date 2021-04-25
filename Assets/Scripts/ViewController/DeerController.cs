using UnityEngine;

public class DeerController : AnimalController
{
    protected virtual void Awake()
    {
        animalModel = new DeerModel();
        base.Awake();
    }
    
    public override string GetObjectLabel()
    {
        return "Deer";
    }
    
    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.30f, 0.30f, 0.30f);
    }

    
}