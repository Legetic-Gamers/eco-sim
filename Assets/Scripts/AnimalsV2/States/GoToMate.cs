/*
 * Authors: Johan A., Alexander L.V.
 */
using UnityEngine;

namespace AnimalsV2.States
{
    public class GoToMate : Wander
    {
        private Vector3 matePos;
        public GoToMate(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

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
            matePos = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Animal");
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