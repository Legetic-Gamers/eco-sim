/*
 * Authors: Alexander L.V, Johan A.
 */
namespace AnimalsV2.States
{
    public class Eating : State
    {

        public Eating(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.LookingOut;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
        }

        public bool foodIsEmpty()
        {
            return true;
        }
    }
}