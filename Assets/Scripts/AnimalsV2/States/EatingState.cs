﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimalsV2.States
{
    public class EatingState : State
    {

        public Action<GameObject> onEatFood;

        private GameObject target;
        
        public EatingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Attack;
            animal.agent.isStopped = true;
            //GetNearestFood();
            EatFood();
            animal.agent.isStopped = false;
        }

        public override void LogicUpdate()
        {
            // base.LogicUpdate();
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

        //public void EatFood(GameObject target)
        public void EatFood()
        {
            onEatFood?.Invoke(target);
            finiteStateMachine.GoToDefaultState();
        }

        public override string ToString()
        {
            return "Eating";
        }

        public override bool MeetRequirements()
        {
            //Vector3 position = animal.transform.position;
            //target = GetNearestFood();
            //if (target == null) return false;
            //bool isCloseEnough = Vector3.Distance(target.transform.position, position) <= 2f;
            //return animal.visibleFoodTargets.Count > 0 && isCloseEnough;
            return target != null;
        }

        // private GameObject GetNearestFood()
        // {
        //     Vector3 position = animal.transform.position;
        //     List<GameObject> nearbyFood = new List<GameObject>();
        //     if (animal.visibleFoodTargets !=null)// first list may be null
        //             nearbyFood=nearbyFood.Concat(animal.visibleFoodTargets).ToList();
        //         if (animal.heardPreyTargets != null)// second list may be null
        //             nearbyFood= nearbyFood.Concat(animal.heardPreyTargets).ToList(); 
        //     
        //     return NavigationUtilities.GetNearestObjectPosition(nearbyFood, position);
        // }
        
        
    }
}