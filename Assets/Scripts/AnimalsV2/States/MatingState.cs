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
            currentStateAnimation = StateAnimation.Mating;
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
            // if (MeetRequirements())
            // {
            //     Mate(GetFoundMate());
            // }
            // else
            // {
            //     finiteStateMachine.GoToDefaultState();
            // }
        }

        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        public IEnumerator Mate()
        {
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();
            
            // make sure target has an AnimalController and that its animalModel is same species
            // if (targetAnimalController != null &&  targetAnimalController.animalModel != null && targetAnimalController.animalModel.IsSameSpecies(animalController.animalModel) &&
            //     targetAnimalController.animalModel.WantingOffspring)
            // {
                //Stop target
                // targetAnimalController.agent.isStopped = true;
                targetAnimalController.waitingState.SetWaitTime(targetAnimalController.matingStateState.matingTime);
                targetAnimalController.fsm.ChangeState(targetAnimalController.waitingState);


                // Wait a while then change state and resume walking
                yield return new WaitForSeconds(matingTime);
                onMate?.Invoke(target);
                
            // }
            
            finiteStateMachine.GoToDefaultState();
            animal.agent.isStopped = false;

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
            yield return null;
        }

        public override string ToString()
        {
            return "Mating";
        }

        // public override bool MeetRequirements()
        // {
        //     return GetFoundMate() != null && FoundMateIsClose();
        // }

        public override bool MeetRequirements()
        {
            return target != null;
            ;
        }

        // private GameObject GetFoundMate()
        // {
        //     List<GameObject> allNearbyFriendly = animal.heardFriendlyTargets.Concat(animal.visibleFriendlyTargets).ToList();
        //
        //     foreach(GameObject potentialMate in allNearbyFriendly)
        //     {
        //         if (potentialMate != null && potentialMate.TryGetComponent(out AnimalController potentialMateAnimalController))
        //         {
        //             // if (potentialMateAnimalController.animalModel.WantingOffspring)
        //             // {
        //                 return potentialMateAnimalController.gameObject;
        //             // }
        //         }
        //     }
        //
        //     return null;
        // }
        //
        // private bool FoundMateIsClose()
        // {
        //     return Vector3.Distance(GetFoundMate().transform.position, animal.transform.position) <= 2f;
        // }
        
        
    }
}