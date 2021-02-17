using UnityEngine;

public class BearController : AnimalController
{
    // Start is called before the first frame update
    new void Start()
    {
        animal = new BearModel();
        base.Start();    // call base class
    }

    
}
