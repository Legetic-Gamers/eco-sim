namespace ViewController
{
    //I know this is subpar design but object label has to be unique for each controller to accomodate object pooler
    public class MLRabbitController : RabbitController
    {
        private AnimalBrainAgent brainAgent;
        protected override void Awake()
        {
            base.Awake();
            if (gameObject.TryGetComponent(out AnimalBrainAgent brainAgent))
            {
                this.brainAgent = brainAgent;
                brainAgent.Init();
            }
        }
        
        public override string GetObjectLabel()
        {
            return "MLRabbit";
        }

        public override void onObjectSpawn()
        {
            base.onObjectSpawn();
            brainAgent.Activate();
        }

        public override void OnObjectDespawn()
        {
            base.OnObjectDespawn();
            brainAgent.Deactivate();
        }
        
    }
}