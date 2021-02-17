using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static FSM.StateAnimation;

namespace FSM
{
    public class FleeingState : FSMState<Animal>
    {

        
        private static readonly FleeingState instance = new FleeingState();

        public static FleeingState Instance
        {
            get { return instance; }
        }

        static FleeingState()
        {
        }

        //Hide constructor
        private FleeingState()
        {
        }
        
        public override void Enter(Animal a)
        {
            currentStateAnimation = Running;
        }

        public override void Execute(Animal a)
        {
            //Get average position of enemies and run away from it.
            Vector3 averagePosition = Utilities.GetNearest(a, "Predator");
            Vector3 pointToRunTo = Utilities.RunToFromPoint(a.transform,averagePosition,false);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            a.nMAgent.SetDestination(hit.position);
        }

        public override void Exit(Animal a)
        {
            
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