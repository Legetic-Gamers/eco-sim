using FSM;
using UnityEngine;

namespace AnimalsV2.States
{
    //Author: Alexander LV, Johan A
    public class Eating : State
    {

        public Eating(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Eating!");
            currentStateAnimation = StateAnimation.Idle;
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