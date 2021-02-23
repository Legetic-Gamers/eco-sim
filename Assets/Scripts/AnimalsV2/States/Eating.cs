/*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using UnityEngine;

namespace AnimalsV2.States
{
    public class Eating : State
    {
        private float timeLeft;
        private bool doneEating;
        public Eating(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            timeLeft = 3.0f;
            doneEating = false;
        }

        public override void Enter()
        {
            base.Enter();
            //Debug.Log("Eating!");
            currentStateAnimation = StateAnimation.Idle;
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
                doneEating = true;
            }   
        }

        public bool foodIsEmpty()
        {
            return doneEating;
        }
    }
}