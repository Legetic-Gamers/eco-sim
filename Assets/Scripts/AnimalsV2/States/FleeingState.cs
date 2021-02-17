/*
 * Authors: Alexander L.V, Johan A.
 */

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
            Debug.Log("ENTERING IDLE");
            currentStateAnimation = Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            Debug.Log("EXECUTING IDLE");
        }

        public void LogicUpdate()
        {
            base.LogicUpdate();
            
            // Get average position of enemies and run away from it.
            Vector3 averagePosition = Utilities.GetNearest(animal, "Predator");
            Vector3 pointToRunTo = Utilities.RunToFromPoint(animal.transform,averagePosition,false);
            
            // Move the animal using the NavMeshAgent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }

        /// <summary>
        /// This function could be extended upon to generate a better point.
        /// this would result in smarter fleeing behavior.
        /// </summary>
        /// <param name="a"> Animal to calculate positions from. </param>
        /// <returns></returns>
        private static Vector3 GetAveragePredatorPosition(Animal a)
        {
            Vector3 averagePosition = new Vector3();
            foreach (GameObject g in a.nearbyObjects)  averagePosition += g.transform.position;
            averagePosition /= a.nearbyObjects.Length;
            return averagePosition;
        }
    }
}