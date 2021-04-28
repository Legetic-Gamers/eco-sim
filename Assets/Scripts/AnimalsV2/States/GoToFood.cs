/*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using System.Collections;
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
            stateAnimation = StateAnimation.Walking;
            
            if (closestFood != null && closestFood.TryGetComponent(out AnimalController a))
            {
                //If we are going to eat an animal
                stateAnimation = StateAnimation.Running;
                //Dont slow down when chasing.
                animal.agent.autoBraking = false;
            }

            //animal.StartCoroutine(ChangeStuckState());
            
            //Make an update instantly
            LogicUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            //Set autobraking back on.
            animal.agent.autoBraking = true;
            
            //animal.StopCoroutine(ChangeStuckState());
        }
        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (closestFood != null)
            {
                Vector3 a = new Vector3(animal.transform.position.x, 0, animal.transform.position.z);
                Vector3 b = new Vector3(closestFood.transform.position.x, 0, closestFood.transform.position.z);


                if (Vector3.Distance(a, b) <= animal.agent.stoppingDistance + 0.75)
                {
                    animal.eatingState.SetTarget(closestFood);
                    finiteStateMachine.ChangeState(animal.eatingState);
                    return;
                }   
            }

            if (MeetRequirements())
            {
                if (animal.agent.isActiveAndEnabled)
                {

                    Vector3 pointToRunTo = closestFood.transform.position;
                    
                    //Overshoot if we are chasing another animal.
                    if (closestFood.TryGetComponent(out NavMeshAgent otherAgent))
                    {
                        pointToRunTo = pointToRunTo + otherAgent.velocity;
                    }

                    //Move the animal using the navmeshagent.
                    bool succesful = NavigationUtilities.NavigateToPoint(animal, pointToRunTo);
                    
                    //if movement was not succesful return to default state;
                    if (!succesful)
                    {
                        //Debug.Log("State gotoFood is stuck, changing to defaultState");
                        finiteStateMachine.GoToDefaultState();
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
            nearbyFood.Clear();
            nearbyFood = animal.heardPreyTargets.Concat(animal.visibleFoodTargets).ToList();
            

            if (animal != null)
            {
                closestFood = NavigationUtilities.GetNearestObject(nearbyFood, animal.transform.position);
            }

            return closestFood != null && !(finiteStateMachine.currentState is EatingState) && !animal.animalModel.HighEnergy;
        }
    }
}