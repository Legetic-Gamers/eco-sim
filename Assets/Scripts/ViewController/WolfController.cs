public class Wolfontroller : AnimalController
{
    // Start is called before the first frame update
    new void Start()
    {
        animal = new WolfModel();
        base.Start();    // call base class
    }

    
}