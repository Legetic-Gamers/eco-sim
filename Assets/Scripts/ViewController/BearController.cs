using UnityEngine;

public class BearController : AnimalController
{
    // Start is called before the first frame update
    void Awake()
    {
        animalModel = new BearModel();
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.2f, 0.2f, 0.2f);
    }
}
