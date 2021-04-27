/*
 * Authors: Johan A.
 */

using System;
using System.Collections;
using UnityEngine;

namespace AnimalsV2.States
{
    public class MatingState : State
    {
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

            stateAnimation = StateAnimation.Attack;

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

        // private void rotateToMate(Transform target)
        // {
        //     RaycastHit hit;
        //     Vector3 direction = target.position - animal.transform.position;
        //     Quaternion targetRotation = transform.rotation;
        //
        //
        //     Quaternion surfaceRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        //     targetRotation = surfaceRotation * transform.rotation;
        //     //Dont rotate around Z.
        //     targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y, 0);
        //
        //
        //     transform.GetChild(0).rotation =
        //         Quaternion.Lerp(transform.GetChild(0).rotation, targetRotation, 2 * Time.deltaTime);
        // }


        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public IEnumerator Mate()
        {
            if (target.TryGetComponent(out AnimalController targetAnimalController))
            {
                //Stop target
                targetAnimalController.waitingState.SetWaitTime(matingTime);
                targetAnimalController.fsm.ChangeState(targetAnimalController.waitingState);


                // Wait a while then change state and resume walking
                yield return new WaitForSeconds(matingTime);
                onMate?.Invoke(target);
                //Debug.Log("Succesfully mated.");
            }

            finiteStateMachine.GoToDefaultState();
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
            yield return null;
        }

        public override string ToString()
        {
            return "Mating";
        }

        public override bool MeetRequirements()
        {
            if (!animal.animalModel.WantingOffspring || !animal.animalModel.IsAlive) return false;

            return target != null && target.TryGetComponent(out AnimalController potentialMateAnimalController) &&
                   potentialMateAnimalController.animalModel.WantingOffspring &&
                   potentialMateAnimalController.animalModel.IsAlive &&
                   !(potentialMateAnimalController.fsm.currentState is MatingState) &&
                   !(potentialMateAnimalController.fsm.currentState is Waiting);
        }
    }
}