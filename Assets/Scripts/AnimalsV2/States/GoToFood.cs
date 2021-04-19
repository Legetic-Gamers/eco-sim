/*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class GoToFood : State
    {
        private List<GameObject> nearbyFood;

        public GameObject closestFood { get; private set; }

        public GoToFood(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
            finiteStateMachine)
        {
            closestFood = null;

            nearbyFood = new List<GameObject>();
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Walking;
            
            if (closestFood != null && closestFood.TryGetComponent(out AnimalController a))
            {
                //If we are going to eat an animal
                currentStateAnimation = StateAnimation.Running;
                //Dont slow down when chasing.
                animal.agent.autoBraking = false;

            }
            
            //Make an update instantly
            LogicUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            //Set autobraking back on.
            animal.agent.autoBraking = true;
        }
        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();


            nearbyFood.Clear();
            //Get all potential food
            if (animal.visibleFoodTargets != null) // first list may be null
                nearbyFood = nearbyFood.Concat(animal.visibleFoodTargets).ToList();
            if (animal.heardPreyTargets != null) // second list may be null
                nearbyFood = nearbyFood.Concat(animal.heardPreyTargets).ToList();
            
            if (MeetRequirements())
            {
                closestFood = NavigationUtilities.GetNearestObject(nearbyFood, animal.transform.position);
                if (closestFood != null && animal.agent.isActiveAndEnabled)
                {

                    Vector3 pointToRunTo = closestFood.transform.position;
                    
                    //Overshoot if we are chasing another animal.
                    //TODO CHANGE TO SOMETHING BETTER.
                    if (closestFood.TryGetComponent(out AnimalController a))
                    {
                        Vector3 diffVec = (pointToRunTo - animal.transform.position).normalized;
                        pointToRunTo = pointToRunTo + diffVec * 3f;
                    }

                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal, pointToRunTo);
                    
                    
                    if (Vector3.Distance(animal.transform.position, closestFood.transform.position) <=
                    animal.agent.stoppingDistance + 0.3)
                    {
                        animal.eatingState.SetTarget(closestFood);
                        finiteStateMachine.ChangeState(animal.eatingState);
                    }
                    
                    
                }else
                {
                    finiteStateMachine.GoToDefaultState();
                }
            }
            else
            {
                finiteStateMachine.GoToDefaultState();
            }
        }

        


        public override string ToString()
        {
            return "Go To Food";
        }

        public override bool MeetRequirements()
        {
            
            nearbyFood = animal.heardPreyTargets.Concat(animal.visibleFoodTargets).ToList();
            

            if (animal != null)
            {
                closestFood = NavigationUtilities.GetNearestObject(nearbyFood, animal.transform.position);
            }

            return closestFood != null && !(finiteStateMachine.currentState is EatingState) && !animal.animalModel.HighEnergy;
        }
    }
}