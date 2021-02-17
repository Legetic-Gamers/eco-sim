using FSM;


//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class SearchForMate : State
    {

        public SearchForMate(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
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