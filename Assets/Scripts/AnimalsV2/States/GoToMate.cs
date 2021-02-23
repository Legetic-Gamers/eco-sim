/*
 * Authors: Johan A., Alexander L.V.
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimalsV2.States
{
    public class GoToMate : State
    {
        private Vector3 matePos;
        public GoToMate(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Searching for mate");
            currentStateAnimation = StateAnimation.Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of mate and run toward it.
            List<GameObject> allPercievedFriendlies =
                animal.visibleFriendlyTargets.Concat(animal.heardFriendlyTargets).ToList();
            matePos = NavigationUtilities.GetNearestObjectPosition(allPercievedFriendlies, animal.transform.position);
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,matePos,true);
            //Move the animal using the navmeshagent.
            UnityEngine.AI.NavMeshHit hit;
            UnityEngine.AI.NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << UnityEngine.AI.NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }
        
        public bool adjacentToMate()
        {
            //return Vector3.Distance(animal.transform.position, foodPos) < 10;
            return false;
        }
    }
}