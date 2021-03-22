/*
 * Authors: Johan A.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimalsV2.States
{
    public class MatingState : State
    {
        private AnimalController animalController;

        public Action<GameObject> onMate;

        private GameObject target;

        public float matingTime = 3.0f;

        public MatingState(AnimalController animalController, FiniteStateMachine finiteStateMachine) : base(
            animalController, finiteStateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();

            currentStateAnimation = StateAnimation.Attack;

            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = true;
            }

            animal.StartCoroutine(Mate());
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Exit()
        {
            base.Exit();
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            animal.StopCoroutine(Mate());
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public IEnumerator Mate()
        {

            if (target.TryGetComponent(out AnimalController targetAnimalController))
            {
                //Stop target
                targetAnimalController.waitingState.SetWaitTime(targetAnimalController.matingState.matingTime);
                targetAnimalController.fsm.ChangeState(targetAnimalController.waitingState);


                // Wait a while then change state and resume walking
                yield return new WaitForSeconds(matingTime);
                onMate?.Invoke(target);    
            }

            finiteStateMachine.GoToDefaultState();
            animal.agent.isStopped = false;

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
            yield return null;
        }

        public override string ToString()
        {
            return "Mating";
        }
        
        public override bool MeetRequirements()
        {
            return target != null && target.TryGetComponent(out AnimalController potentialMateAnimalController) &&
                   potentialMateAnimalController.animalModel.WantingOffspring &&
                   potentialMateAnimalController.animalModel.IsAlive && !(potentialMateAnimalController.fsm.currentState is MatingState);
        }

        
        
        
    }
}