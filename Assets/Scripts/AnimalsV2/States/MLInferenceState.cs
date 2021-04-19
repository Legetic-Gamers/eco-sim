namespace AnimalsV2.States
{
    public class MLInferenceState : State
    {
        public MLInferenceState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            //currentStateAnimation = StateAnimation.Idle;
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            //Debug.Log("EXECUTING IDLE");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }
        
        public override string ToString()
        {
            return "ML Inference";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}