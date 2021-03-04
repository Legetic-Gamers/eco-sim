/*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class GoToFood : State
    {
        public GoToFood(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Running;
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
                GameObject closestFood = NavigationUtilities.GetNearestObjectPosition(animal.visibleFoodTargets, animal.transform.position);
                if (closestFood != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo =
                        NavigationUtilities.RunToFromPoint(animal.transform, closestFood.transform.position, true);
                    //Move the animal using the navmeshagent.
                    NavMeshHit hit;
                    NavMesh.SamplePosition(pointToRunTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
                    animal.agent.SetDestination(hit.position);
                    if (Vector3.Distance(animal.gameObject.transform.position, closestFood.transform.position) <= 2f)
                    {
                        finiteStateMachine.ChangeState(animal.eatingState);
                    }    
                }
                
            }
            else
            {
                finiteStateMachine.ChangeState(animal.wanderState);
            }
        }
        
        

        public override string ToString()
        {
            return "Go To Food";
        }

        public override bool MeetRequirements()
        {
            return animal.visibleFoodTargets.Count > 0 && !(finiteStateMachine.CurrentState is EatingState);
        }
    }
}