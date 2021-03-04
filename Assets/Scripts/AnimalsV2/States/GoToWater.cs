using UnityEngine.AI;

namespace AnimalsV2.States
{
    using System;
    using UnityEngine;

    namespace AnimalsV2.States
    {
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
                    GameObject closestWater = NavigationUtilities.GetNearestObjectPosition(animal.visibleWaterTargets, animal.transform.position);
                    if (closestWater != null && animal.agent.isActiveAndEnabled)
                    {
                        Vector3 pointToRunTo =
                            NavigationUtilities.RunToFromPoint(animal.transform, closestWater.transform.position, true);
                        //Move the animal using the navmeshagent.
                        NavMeshHit hit;
                        NavMesh.SamplePosition(pointToRunTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
                        animal.agent.SetDestination(hit.position);
                        if (Vector3.Distance(hit.position, closestWater.transform.position) <= 2f)
                        {
                            finiteStateMachine.ChangeState(animal.drinkingState);
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
                return "Going to water";
            }

            public override bool MeetRequirements()
            {
                // rewuirements for this state are following
                return animal.visibleWaterTargets.Count > 0 && !(finiteStateMachine.CurrentState is DrinkingState);
            }
        }
    }
}