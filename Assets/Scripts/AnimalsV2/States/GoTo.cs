using UnityEngine;

namespace AnimalsV2.States
{
    public class GoTo : State
    {
        private void GoTo(Vector3 toPoint)
        {
            /*toPoint = NavigationUtilities.GetNearestObjectPosition(animal.visiblePreyTargets, animal.transform.position);
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,foodPos,true);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);*/
        }

        public GoTo(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) { }
    }
}