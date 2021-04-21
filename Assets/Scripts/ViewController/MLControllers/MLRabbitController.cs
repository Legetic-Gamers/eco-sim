namespace ViewController
{
    //I know this is subpar design but object label has to be unique for each controller to accomodate object pooler
    public class MLRabbitController : RabbitController
    {
        public override string GetObjectLabel()
        {
            return "MLRabbit";
        }

        public override void onObjectSpawn()
        {
            base.onObjectSpawn();
            
            if (gameObject.TryGetComponent(out AnimalBrainAgent brainAgent))
            {
                brainAgent.Init();
            }
        }
    }
}