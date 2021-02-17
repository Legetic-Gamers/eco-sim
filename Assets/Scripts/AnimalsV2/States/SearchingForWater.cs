using FSM;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class SearchingForWater : State
    {

        public SearchingForWater(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
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
