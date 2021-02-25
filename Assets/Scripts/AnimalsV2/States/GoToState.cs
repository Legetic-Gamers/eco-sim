using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.StateAnimation;

namespace AnimalsV2.States
{
    public class GoToState : State
    {
        private GameObject targetObject;
        private string action;
        
        //TODO fix so that if target is out of range, stop trying to go to that target. E.g. if a rabbit succesfully flees a wolf, wolf should not keep having rabbit as target

        public GoToState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            
        }
        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = Running;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (targetObject != null)
            {
                Vector3 pointToRunTo =
                    NavigationUtilities.RunToFromPoint(animal.transform, targetObject.transform.position, true);
                //Move the animal using the navmeshagent.
                NavMeshHit hit;
                NavMesh.SamplePosition(pointToRunTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
                animal.agent.SetDestination(hit.position);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public void SetTarget(GameObject t)
        {
            targetObject = t;
        }

        public void SetActionOnArrive(string action)
        {
            this.action = action;
        }
        
        public GameObject GetTarget()
        {
            return targetObject;
        }

        public string GetAction()
        {
            return action;
        }
        public bool arrivedAtTarget()
        {
            if (animal != null && targetObject != null)
            {
                return Vector3.Distance(animal.transform.position, targetObject.transform.position) < 1f;
            }

            return false;
        }
        
        public override string ToString()
        {
            return "Going to target";
        }
    }
}