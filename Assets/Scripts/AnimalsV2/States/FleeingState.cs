using System;
using System.Collections.Generic;
using AnimalsV2;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.StateAnimation;

namespace AnimalsV2.States
{
    public class FleeingState : State
    {
        private Vector3 averagePosition;
        public FleeingState(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public Vector3 fleeingFromPos;
        
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Fleeing!");
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
            //averagePosition = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Predator");
            
            // alternative to the above
            averagePosition = fleeingFromPos;
            
            //Run run away from the position.
            //Default to just running forward.
            Vector3 pointToRunTo = animal.transform.position + animal.transform.forward;
            //If there was nothing to run from found.
            if (averagePosition != animal.transform.position)
            {
                 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,averagePosition,false);
            }
            
            // Move the animal using the NavMeshAgent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }


    }
}