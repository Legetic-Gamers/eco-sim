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
        
        
        //Fields
        private NavMeshAgent nMAgent;
        
        
        public override void Enter(Animal a)
        {
            nMAgent = a.GetComponent<NavMeshAgent>();
            currentStateAnimation = Running;
        }

        public override void Execute(Animal a)
        {
            //Get average position of enemies and run away from it.
            Vector3 averagePosition = GetClosestPredatorPosition(a);
            RunToFromPoint(a.transform,averagePosition,false);
        }
        

        public override void Exit(Animal a)
        {
            
        }
        
        //Uses
        //Inspired by: https://answers.unity.com/questions/868003/navmesh-flee-ai-flee-from-player.html 
        private void RunToFromPoint(Transform animalTransform,Vector3 targetPoint,bool toPoint)
        {
            
            Vector3 pointToAnimalVector;

            //Run to point or from point
            if (toPoint)
            {
                pointToAnimalVector = targetPoint - animalTransform.position;
            }
            else
            {
                pointToAnimalVector = animalTransform.position - targetPoint;
            }

            //Calculate point to run towards or from.
            Vector3 pointToRunTo = animalTransform.position + Vector3.Normalize(pointToAnimalVector);
            
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            nMAgent.SetDestination(hit.position);

        }
        
        //This function could be extended upon to generate a better point.
        //This would result in smarter fleeing behavior.
        private static Vector3 GetAveragePredatorPosition(Animal a)
        {
            Vector3 averagePosition = new Vector3();
            foreach (GameObject g in a.nearbyPredators)
            {
                averagePosition += g.transform.position;
            }

            averagePosition /= a.nearbyPredators.Length;

            return averagePosition;
        }
        
        private static Vector3 GetClosestPredatorPosition(Animal a)
        {
            Vector3 animalPosition = a.transform.position;
            if (a.nearbyPredators.Length == 0) return animalPosition;
            
            
            Vector3 closestPredatorPostiion = a.nearbyPredators[0].transform.position;
            float closestDistance = Vector3.Distance(closestPredatorPostiion, animalPosition);
            
            foreach (GameObject g in a.nearbyPredators)
            {
                float dist = Vector3.Distance(g.transform.position, animalPosition);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestPredatorPostiion = g.transform.position;
                }
                
            }
            
            return closestPredatorPostiion;
        }

    }
}