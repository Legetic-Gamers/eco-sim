using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AnimalsV2;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.StateAnimation;

namespace AnimalsV2.States
{
    public class FleeingState : State
    {
        private Vector3 averagePosition;
        public FleeingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}
        
        private bool hasFled;
        
        public override void Enter()
        {
            base.Enter();
            hasFled = false;
            //Debug.Log("Fleeing!");
            currentStateAnimation = Running;
        }

        public override void Exit()
        {
            base.Exit();
            //Debug.Log("EXITING FLEEING");
            //currentStateAnimation = StateAnimation.Idle;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // Get average position of enemies
            List<GameObject> allHostileTargets = animal.heardHostileTargets.Concat(animal.visibleHostileTargets).ToList();

            averagePosition = NavigationUtilities.GetNearObjectsAveragePosition(allHostileTargets, animal.transform.position);
            
            //Run run away from the position.
            //Default to just running forward.
            Vector3 pointToRunTo = animal.transform.position + animal.transform.forward;
            //If there was nothing to run from found.
            if (averagePosition != animal.transform.position)
            {
                 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,averagePosition,false);
            }
            else
            {
                Task.Run(async () =>
                {
                    await Task.Delay(2000);
                    finiteStateMachine.ChangeState(animal.wanderState);
                });
            }

            if (animal.agent.isActiveAndEnabled)
            {
                // Move the animal using the NavMeshAgent.
                NavMeshHit hit;
                NavMesh.SamplePosition(pointToRunTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
                animal.agent.SetDestination(hit.position);
            }
        }

        public override string ToString()
        {
            return "Fleeing";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}