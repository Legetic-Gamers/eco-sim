using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimalsV2.States
{

    public class EatingState : State
    {

        public Action<GameObject, float> onEatFood;
        private GameObject target;

        public EatingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
            finiteStateMachine)
        {
            stateAnimation = StateAnimation.Attack;
        }

        public override void Enter()
        {
            base.Enter();

            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = true;
            }

            animal.StartCoroutine(EatFood());
        }

        public override void Exit()
        {
            base.Exit();
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            
            animal.StopCoroutine(EatFood());
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        //New
        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        private IEnumerator EatFood()
        {
            
            
            
            // Wait a while then eat, change state and resume walking
            if (target.TryGetComponent(out AnimalController animalController))
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(1);   //Outcommented for training ml  
            }
            
            onEatFood?.Invoke(target, animal.animalModel.currentEnergy);


            //Eat the food
            //The reason to why I have curentEnergy as an in-parameter is because currentEnergy is updated through EatFood before reward gets computed in AnimalMovementBrain
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            //Debug.Log("Succesfully ate.");
            
            finiteStateMachine.GoToDefaultState();
            yield return null;

        }

        public override string ToString()
        {
            return "Eating";
        }

        public override bool MeetRequirements()
        {
            return target != null && !animal.animalModel.HighEnergy;
        }

    }
}