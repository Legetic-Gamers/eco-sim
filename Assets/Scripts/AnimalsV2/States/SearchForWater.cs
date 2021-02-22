/*
 * Author: Johan A, Alexander L.V.
 */

using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.StateAnimation;


namespace AnimalsV2.States
{
    public class SearchForWater : Wander
    {
        private Vector3 waterPos;
        public SearchForWater(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = Running;
            Debug.Log("Searching for water!");
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of water and run toward it.
            waterPos = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Water");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,waterPos,true);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }
        
        public bool adjacentToWater()
        {
            //return Vector3.Distance(animal.transform.position, foodPos) < 10;
            return false;
        }
    }
}