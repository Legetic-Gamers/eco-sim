using FSM;

namespace AnimalsV2.States
{
    public class Eating : State
    {

        public Eating(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
        }
    }
}