/*
 * Author: Johan A.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class Wander : State 
    {
        private List<State> priorities = new List<State>();
        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            foreach (var VARIABLE in priorities)
            {
                
            }
        }

        public void SetPriorities(List<State> priorities)
        {
            this.priorities = priorities;
        }
    }
}