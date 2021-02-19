/*
 * Authors: Johan A.
 */

using UnityEngine;

namespace AnimalsV2.States
{
    public class Mating : State
    {
    
        private float timeLeft = 3.0f;
        public Mating(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

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
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                // Go back to Wander
            }
        }
    }
}