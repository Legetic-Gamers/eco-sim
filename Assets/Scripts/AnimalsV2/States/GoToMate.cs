using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class GoToMate : State
    {
        public GoToMate(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (MeetRequirements())
            {
                GameObject foundMate = GetFoundMate();
                if (foundMate != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform, foundMate.transform.position, true);
                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal,pointToRunTo);
                    
                }
                
            }
            else
            {
                finiteStateMachine.GoToDefaultState();
            }
        }
        
        

        public override string ToString()
        {
            return "Going To Mate";
        }

        public override bool MeetRequirements()
        {
            return animal.heardFriendlyTargets.Concat(animal.visibleFriendlyTargets).ToList().Count > 0 && !(finiteStateMachine.currentState is MatingState) && animal.animalModel.WantingOffspring && GetFoundMate() != null;
        }

        private GameObject GetFoundMate()
        {
            List<GameObject> allNearbyFriendly = animal.heardFriendlyTargets.Concat(animal.visibleFriendlyTargets).ToList();
            //Debug.Log("Nfriendly" + allNearbyFriendly.Count);
            foreach(GameObject potentialMate in allNearbyFriendly)
            {
                if (potentialMate != null && potentialMate.TryGetComponent(out AnimalController potentialMateAnimalController) && potentialMateAnimalController.animalModel.WantingOffspring && potentialMateAnimalController.animalModel.IsAlive && !(potentialMateAnimalController.fsm.currentState is MatingState))
                {
                    
                    return potentialMateAnimalController.gameObject;
                }
            }

            return null;
        }
    }
}