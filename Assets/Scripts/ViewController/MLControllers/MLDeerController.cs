namespace ViewController
{
    //I know this is subpar design but object label has to be unique for each controller to accomodate object pooler
    public class MLDeerController : DeerController
    {
        public override string GetObjectLabel()
        {
            return "MLDeer";
        }
    }
}