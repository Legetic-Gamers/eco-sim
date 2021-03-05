/*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class GoToFood : State
    {
        private List<GameObject> nearbyFood;
        public GoToFood(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Walking;
            nearbyFood = new List<GameObject>();
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (MeetRequirements())
            {
                if (animal.visibleFoodTargets !=null)// first list may be null
                    nearbyFood=nearbyFood.Concat(animal.visibleFoodTargets).ToList();
                if (animal.heardPreyTargets != null)// second list may be null
                    nearbyFood= nearbyFood.Concat(animal.heardPreyTargets).ToList(); 
                
                GameObject closestFood = NavigationUtilities.GetNearestObjectPosition(nearbyFood, animal.transform.position);
                if (closestFood != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform, closestFood.transform.position, true);
                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal,pointToRunTo);
                    // NavMeshHit hit;
                    // NavMesh.SamplePosition(pointToRunTo, out hit, 100, 1 << NavMesh.GetAreaFromName("Walkable"));
                    // animal.agent.SetDestination(hit.position);
                    if (Vector3.Distance(animal.transform.position, closestFood.transform.position) <= 2f)
                    {
                        finiteStateMachine.ChangeState(animal.eatingState);
                    }  
                    
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
            return nearbyFood.Count > 0 && !(finiteStateMachine.CurrentState is EatingState);
        }
    }
}