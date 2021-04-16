namespace AnimalsV2.States
{
    public class MLState : State
    {
        public MLState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
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
            return "MLState";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}