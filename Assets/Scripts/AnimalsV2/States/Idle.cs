using FSM;
using UnityEngine;

namespace AnimalsV2.States
{
    public class Idle : State
    {

        public Idle(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
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