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
        private float distance;
        private NavMeshAgent nMAgent;
        
        
        public override void Enter(Animal a)
        {
            nMAgent = a.GetComponent<NavMeshAgent>();
            currentStateAnimation = Running;
        }

        public override void Execute(Animal a)
        {
            //Get average position of enemies and run away from it.
            Vector3 averagePosition = GetAveragePredatorPosition(a);
            RunAway(a.transform,averagePosition);
        }
        

        public override void Exit(Animal a)
        {
            
        }
        
        //Inspired by: https://answers.unity.com/questions/868003/navmesh-flee-ai-flee-from-player.html 
        private void RunAway(Transform animalTransform,Vector3 averageEnemyPosition)
        {
            Transform startTransform = animalTransform;

            animalTransform.rotation = Quaternion.LookRotation(animalTransform.position - averageEnemyPosition);

            Vector3 pointToRunTo = animalTransform.position + animalTransform.forward * distance;
            
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            
            //Reset transform
            animalTransform.position = startTransform.position;
            animalTransform.rotation = startTransform.rotation;
            
        }
        
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

    }
}