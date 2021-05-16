using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AnimalsV2;
using UnityEngine;
using UnityEngine.AI;
using ViewController;
using static AnimalsV2.StateAnimation;

namespace AnimalsV2.States
{
    public class FleeingState : State
    {
        private Vector3 averagePosition;

        private string stateName = "FleeingState";

        public FleeingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
            finiteStateMachine)
        {
            stateAnimation = Running;
        }
        
        
        public override void Enter()
        {
            base.Enter();
            float fleeTime;

            if (IsSeenByHostile())
            {
                if (IsBeingChased())
                {
                    stateName = "Fleeing";
                    fleeTime = 3f;
                } else {
                    stateName = "Evading vision";
                    fleeTime = 1.5f;   
                }
            }
            else
            {
                stateName = "Avoiding hostile";
                fleeTime = 1f;
            }
            
            animal.StartCoroutine(ReturnToDefaultStateAfterDelay(fleeTime));
            //Make an update instantly
            LogicUpdate();
        }

        public override void Exit()
        {
            base.Exit();
            animal.StopCoroutine(ReturnToDefaultStateAfterDelay(0));
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // If a hideout is found try to hide
            GameObject hideout = NavigationUtilities.GetNearestObject(animal.visibleHideoutTargets, animal.transform.position);
            if (hideout && hideout.TryGetComponent(out HideoutController hideoutController) && hideoutController.CanHide(animal))
            {
                NavigationUtilities.NavigateToPoint(animal, hideout.transform.position);
                
                if (Vector3.Distance(animal.transform.position, hideout.transform.position) < 1f)
                {
                    animal.hiding.SetTarget(hideoutController);
                    finiteStateMachine.ChangeState(animal.hiding);
                }
                return;
            }




            // Get average position of enemies
            List<GameObject> allHostileTargets = animal.heardHostileTargets.Concat(animal.visibleHostileTargets).ToList();
                
            averagePosition = NavigationUtilities.GetNearObjectsAveragePosition(allHostileTargets, animal.transform.position);
            
            //Run run away from the position.
            //Default to just running forward.
            Vector3 pointToRunTo = animal.transform.position + animal.transform.forward * 5f;
            
            //If we found a hostile averagePosition we set new vector and reset timer
            if (averagePosition != animal.transform.position)
            {
                pointToRunTo = NavigationUtilities.RunFromPoint(animal.transform,averagePosition);
            }

            //Move agent
            if (animal.agent.isActiveAndEnabled)
            {
                // Move the animal using the NavMeshAgent.
                NavMeshHit hit;
                if (NavMesh.SamplePosition(pointToRunTo, out hit, animal.agent.height*2, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    NavigationUtilities.NavigateToPoint(animal, hit.position);

                }//Try running perpendicular to front (Avoid walls).
                else if (NavigationUtilities.PerpendicularPoint(animal.transform.position,animal.transform.forward,animal.transform.up,animal.agent.height*2 + 2f,out pointToRunTo))
                {
                    NavigationUtilities.NavigateToPoint(animal, pointToRunTo);

                } //Try running randomly if no other way found.
                else if(NavigationUtilities.RandomPoint(animal.transform.position, 10f,10f, out pointToRunTo))
                {
                    NavigationUtilities.NavigateToPoint(animal, pointToRunTo);
                }
            }

        }

        IEnumerator ReturnToDefaultStateAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            //if no hostiles go back to default state
            if (!MeetRequirements())
            {
                finiteStateMachine.GoToDefaultState();
            }
            else if(finiteStateMachine.currentState == this)    //make sure that enter does not get called if state has changed since coroutine started (such as changing to hiding)
            {
                Enter();
            }
        }

        private bool IsBeingChased()
        {
            foreach (GameObject hostile in animal.heardHostileTargets.Concat(animal.visibleHostileTargets))
            {
                if (hostile.TryGetComponent(out AnimalController hostileAnimalController) && hostileAnimalController.fsm.currentState is GoToFood)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsSeenByHostile()
        {
            foreach (GameObject hostile in animal.heardHostileTargets.Concat(animal.visibleHostileTargets))
            {
                if (hostile.TryGetComponent(out AnimalController hostileAnimalController) && hostileAnimalController.visibleFoodTargets.Contains(animal.gameObject))
                {
                    return true;
                }
            }

            return false;
        }
        

        public override string ToString()
        {
            return stateName;
        }

        public override bool MeetRequirements()
        {
            return animal.heardHostileTargets.Concat(animal.visibleHostileTargets).ToList().Count > 0 && !animal.animalModel.CriticalHydration && !animal.animalModel.CriticalEnergy;
        }
    }
}