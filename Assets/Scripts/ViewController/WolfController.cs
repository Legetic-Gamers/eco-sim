using UnityEngine;

public class WolfController : AnimalController
{
    void Awake()
    {
        animalModel = new WolfModel();
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.3f, 0.3f, 0.3f);
    }
}