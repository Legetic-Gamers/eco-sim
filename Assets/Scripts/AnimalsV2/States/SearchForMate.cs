﻿/*
 * Authors: Johan A., Alexander L.V.
 */

using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class SearchForMate : Wander
    {
        private Vector3 matePos;
        public SearchForMate(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Running;
        }

        public void HandleInput()
        {
            base.HandleInput();
        }

        public void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of mate and run toward it.
            matePos = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Animal");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,matePos,true);
            //Move the animal using the navmeshagent.
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }
    }
}