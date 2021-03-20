using UnityEngine;

namespace AnimalsV2.States
{
    public class Idle : State
    {

        public Idle(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            //currentStateAnimation = StateAnimation.Idle;
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
            //Debug.Log("Idling!");
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

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}