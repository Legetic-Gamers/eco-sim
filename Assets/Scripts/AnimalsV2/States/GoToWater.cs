using UnityEngine.AI;

namespace AnimalsV2.States
{
    using System;
using UnityEngine;

    public class GoToWater : State
    {

        
        public GoToWater(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Walking;
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
                GameObject closestWater = NavigationUtilities.GetNearestObject(animal.visibleWaterTargets, animal.transform.position);
                if (closestWater != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo = closestWater.transform.position;
                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal,pointToRunTo);
                    // NavMeshHit hit;
                    // NavMesh.SamplePosition(pointToRunTo, out hit, 100, 1 << NavMesh.GetAreaFromName("Walkable"));
                    // animal.agent.SetDestination(hit.position);
                    // if (animal is WolfController)
                    // {
                    //     Debug.Log("Water, Remaining: " + animal.agent.remainingDistance + " Stopping: " + animal.agent.stoppingDistance + " True?: " + (animal.agent.remainingDistance <= animal.agent.stoppingDistance));
                    // }

                    if(Vector3.Distance(animal.transform.position, closestWater.transform.position) <= animal.agent.stoppingDistance){
                        animal.drinkingState.SetTarget(closestWater);
                        finiteStateMachine.ChangeState(animal.drinkingState);
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
            return "Go to water";
        }

        public override bool MeetRequirements()
        {
            // rewuirements for this state are following
            return animal.visibleWaterTargets.Count > 0 && !(finiteStateMachine.CurrentState is DrinkingState);
        }
    }
}
