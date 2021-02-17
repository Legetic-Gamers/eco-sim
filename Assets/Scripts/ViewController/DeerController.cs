public class DeerController : AnimalController
{
    // Start is called before the first frame update
    new void Start()
    {
        animal = new DeerModel();
        base.Start();    // call base class
    }

    
}