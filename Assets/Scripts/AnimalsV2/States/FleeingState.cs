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

        public FleeingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal,
            finiteStateMachine)
        {
            currentStateAnimation = Running;
        }

        // own timer (note it's unit is number of LogicalUpdate ticks. This is the number of ticks in which the state will hold after MeetRequirements becomes false. We want the animal to run a little more than just outside of percieved predators space
        private float timer;
        private const float startTimerValue = 1f;
        
        public override void Enter()
        {
            base.Enter();
            timer = startTimerValue;
        }

        public override void Exit()
        {
            base.Exit();
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
            
            //If we found a hostile averagePosition we set new vector and reset timer
            if (averagePosition != animal.transform.position)
            {
                timer = startTimerValue;
                pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,averagePosition,false);
            }
            else
            {
                timer-=Time.deltaTime;
            }

            if (animal.agent.isActiveAndEnabled)
            {
                // Move the animal using the NavMeshAgent.
                NavMeshHit hit;
                NavMesh.SamplePosition(pointToRunTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
                animal.agent.SetDestination(hit.position);
            }

            // if timer has ran out, we change to default state
            if (timer <= 0)
            {
                finiteStateMachine.GoToDefaultState();
            }

        }

        public override string ToString()
        {
            return "Fleeing";
        }

        public override bool MeetRequirements()
        {
            return animal.heardHostileTargets.Concat(animal.visibleHostileTargets).ToList().Count > 0;
        }
    }
}