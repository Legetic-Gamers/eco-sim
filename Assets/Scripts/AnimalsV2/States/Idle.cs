using UnityEngine;

namespace AnimalsV2.States
{
    public class Idle : State
    {

        public Idle(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine){}

        public override void Enter()
        {
            base.Enter();
            //Debug.Log("Idling!");
            currentStateAnimation = StateAnimation.Idle;
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
            return "Idle";
        }
    }
}