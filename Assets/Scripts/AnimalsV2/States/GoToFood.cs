/*
 * Authors: Johan A., Alexander L.V.
 */
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class GoToFood : Wander
    {
        private Vector3 foodPos;
        public GoToFood(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Searching For Food");
            currentStateAnimation = StateAnimation.Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of enemies and run away from it.
            foodPos = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Food");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,foodPos,true);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }

        public bool adjacentToFood()
        {
            if (foodPos != null)
            {
               //return Vector3.Distance(animal.transform.position, foodPos) < 10;
            }
            else
            {
                return false;
            }

            return false;

        }
    }
}