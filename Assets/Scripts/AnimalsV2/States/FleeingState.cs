using System.Collections.Generic;
using AnimalsV2;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using static FSM.StateAnimation;

namespace AnimalsV2.States
{
    public class FleeingState : State
    {

        public FleeingState(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("ENTERING FLEEING");
            currentStateAnimation = Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            //Debug.Log("EXECUTING FLEEING");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of enemies and run away from it.
            Vector3 averagePosition = Utilities.GetNearest(animal, "Predator");
            Vector3 pointToRunTo = Utilities.RunToFromPoint(animal.transform,averagePosition,false);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }

        //This function could be extended upon to generate a better point.
        //This would result in smarter fleeing behavior.
        private static Vector3 GetAveragePredatorPosition(Animal a)
        {
            Vector3 averagePosition = new Vector3();
            foreach (GameObject g in a.nearbyObjects)
            {
                averagePosition += g.transform.position;
            }

            averagePosition /= a.nearbyObjects.Length;

            return averagePosition;
        }
    }
}