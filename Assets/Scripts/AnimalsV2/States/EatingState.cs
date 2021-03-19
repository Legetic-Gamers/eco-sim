﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimalsV2.States
{

    public class EatingState : State
    {

        public Action<GameObject> onEatFood;
        private GameObject target;

        public EatingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
            finiteStateMachine)
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
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // if (MeetRequirements())
            // {
            //     EatFood(target);
            // }
            // else
            // {
            //     finiteStateMachine.GoToDefaultState();
            // }
        }

        //New
        public void SetTarget(GameObject target)
        {
            this.target = target;
        }

        private IEnumerator EatFood()
        {
            //Eat the food
            onEatFood?.Invoke(target);
            
            
            // Wait a while then change state and resume walking
            yield return new WaitForSeconds(1);
            finiteStateMachine.GoToDefaultState();
            animal.agent.isStopped = false;

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