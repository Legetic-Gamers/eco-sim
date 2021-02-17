using FSM;
using UnityEngine;

namespace AnimalsV2.States
{
    public class Idle : State
    {

        public Idle(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            //Debug.Log("ENTERING IDLE");
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
    }
}