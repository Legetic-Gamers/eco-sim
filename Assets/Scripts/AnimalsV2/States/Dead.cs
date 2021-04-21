using System;
using UnityEngine;

namespace AnimalsV2.States
{
    namespace AnimalsV2.States
    {
        public class Dead : State
        {

            public Action<AnimalController, bool> onDeath;
            
            public Dead(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
                finiteStateMachine)
            {
                stateAnimation = StateAnimation.Dead;
            }

            public override void Enter()
            {
                
                //when entering state dead,
                base.Enter();
                if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
                {
                    animal.agent.isStopped = true;
                }
                // Set state so that it can't change
                finiteStateMachine.absorbingState = true;
                onDeath?.Invoke(animal, false);
            }

            public override void HandleInput()
            {
                base.HandleInput();
            }

            public override void LogicUpdate()
            {
                base.LogicUpdate();
            }

            public override string ToString()
            {
                return "Dead";
            }

            public override bool MeetRequirements()
            {
                return true;
            }
        }
    }
}