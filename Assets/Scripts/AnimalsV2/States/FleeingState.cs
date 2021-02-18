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
        public FleeingState(Animal animal, StateMachine stateMachine) : base(animal, stateMachine) {}

        public override void Enter()
        {
            base.Enter();
            Debug.Log("ENTERING FLEEING");
            currentStateAnimation = Running;
        }

        public override void Exit()
        {
            base.Exit();
            Debug.Log("EXITING FLEEING");
            //currentStateAnimation = StateAnimation.Idle;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            //Debug.Log("EXECUTING FLEEING");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            // Get average position of enemies and run away from it.
            Vector3 averagePosition = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Predator");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,averagePosition,false);
            
            // Move the animal using the NavMeshAgent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }

        
    }
}