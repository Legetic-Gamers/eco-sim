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
            currentStateAnimation = StateAnimation.Attack;
        }

        public override void Enter()
        {
            base.Enter();

            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = true;
            }

            //GetNearestFood();
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
            
            //Eat the food
            //The reason to why I have curentEnergy as an in-parameter is because currentEnergy is updated through EatFood before reward gets computed in AnimalMovementBrain
            onEatFood?.Invoke(target, animal.animalModel.currentEnergy);
            
            // Wait a while then change state and resume walking
            yield return new WaitForSeconds(1/Time.timeScale);
            
            
            if (animal.agent.isActiveAndEnabled && animal.agent.isOnNavMesh)
            {
                animal.agent.isStopped = false;
            }
            //Debug.Log("Succesfully ate.");
            
            finiteStateMachine.GoToDefaultState();

            // Very important, this tells Unity to move onto next frame. Everything crashes without this
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