namespace ViewController
{
    //I know this is subpar design but object label has to be unique for each controller to accomodate object pooler
    public class MLBearController : BearController
    {
        private AnimalBrainAgent brainAgent;
        private void Awake()
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
            return "MLBear";
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