using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.StateAnimation;

namespace AnimalsV2.States
{
    public class GoToState : State
    {
        private GameObject targetObject;
        private Priorities action;
        
        public GoToState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            
        }
        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = Walking;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (animal.agent.isActiveAndEnabled && targetObject != null)
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
            
            //Override animation
            if (t != null)
            {
                
                AnimalController targetController = t.GetComponent<AnimalController>();
                //target is an animal and i can eat it -> we are chasing something.
                if (targetController != null && animal.animalModel.CanEat(targetController.animalModel)){
                    //Run fast if chasing
                    currentStateAnimation = Running;
                    Debug.Log("Chasing!");
                    
                }
            }
        }

        public void SetActionOnArrive(Priorities action)
        {
            this.action = action;
        }
        
        public GameObject GetTarget()
        {
            return targetObject;
        }

        public Priorities GetAction()
        {
            return action;
        }
        public bool arrivedAtTarget()
        {
            if (animal != null && targetObject != null)
            {
                // This is still not perfect. Atleast we check the reach radius of the animal, and apply a distance of 1f. This will cause problems if the target has a radius larger than 1f
                return Vector3.Distance(animal.transform.position, targetObject.transform.position) < (animal.GetComponent<CharacterController>().radius + 1f);
            }

            return false;
        }
        
        public override string ToString()
        {
            return "Going to target";
        }
    }
}