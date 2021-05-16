namespace AnimalsV2.States
{
    public class MLTrainingState : State
    {
        public MLTrainingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            //currentStateAnimation = StateAnimation.Idle;
            stateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
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
            return "ML Training";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}