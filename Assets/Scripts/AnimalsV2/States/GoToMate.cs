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
            
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Walking;
            
            //Make an update instantly
            LogicUpdate();
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
                    Vector3 pointToRunTo = foundMate.transform.position;
                    
                    // if(foundMate.TryGetComponent(out AnimalController otherAnimalController))
                    // {
                    //     if (otherAnimalController.fsm.currentState is Wander)
                    //     {
                    //         otherAnimalController.fsm.ChangeState(otherAnimalController.waitingState);
                    //     }
                    // }
                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal,pointToRunTo);
                    
                    
                    if (Vector3.Distance(animal.transform.position, foundMate.transform.position) <= animal.agent.stoppingDistance + 0.3)
                    {
                        animal.matingState.SetTarget(foundMate);
                        //Try to change state, else go to default state
                        if (!finiteStateMachine.ChangeState(animal.matingState))
                        {
                            finiteStateMachine.GoToDefaultState();
                        }
                        
                    }    
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
            return  !(finiteStateMachine.currentState is MatingState) && animal.animalModel.WantingOffspring && GetFoundMate() != null;
        }

        public GameObject GetFoundMate()
        {
            List<GameObject> allNearbyFriendly = animal.heardFriendlyTargets.Concat(animal.visibleFriendlyTargets).ToList();
            //Debug.Log("Nfriendly" + allNearbyFriendly.Count);
            foreach(GameObject potentialMate in allNearbyFriendly)
            {
                if (potentialMate != null && potentialMate.TryGetComponent(out AnimalController potentialMateAnimalController) && potentialMateAnimalController.animalModel.IsAlive)
                {
                    
                    return potentialMateAnimalController.gameObject;
                }
            }

            return null;
        }
    }
}