/*
 * Authors: Johan A.
 */

using System;
using UnityEngine;

namespace AnimalsV2.States
{
    public class Mating : State
    {
        private AnimalController animalController;

        public Action<GameObject> onMate;
        
        private float timeLeft = 3.0f;
        public Mating(AnimalController animalController, FiniteStateMachine finiteStateMachine) : base(animalController, finiteStateMachine) {}

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

        public void Mate(GameObject target)
        {
            onMate.Invoke(target);
        }
        
        public override string ToString()
        {
            return "Mating";
        }
    }
}