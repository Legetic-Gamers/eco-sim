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
            stateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
            
            //Dont slow down when chasing.
            animal.agent.autoBraking = false;
            
            //Make an update instantly
            LogicUpdate();
        }
        
        public override void Exit()
        {
            base.Exit();
            
            animal.agent.autoBraking = true;
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
                    NavigationUtilities.NavigateToPoint(animal, pointToRunTo);

                    //Stop the other rabbit
                    // if (foundMate.TryGetComponent(out AnimalController targetAnimalController))
                    // {
                    //     targetAnimalController.waitingState.SetWaitTime(2);
                    //     targetAnimalController.fsm.ChangeState(targetAnimalController.waitingState);
                    // }


                    Vector3 a = new Vector3(animal.transform.position.x, 0, animal.transform.position.z);
                    Vector3 b = new Vector3(foundMate.transform.position.x, 0, foundMate.transform.position.z);
                    if (Vector3.Distance(a, b) <= animal.agent.stoppingDistance + 1.2f)
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
                if (potentialMate != null && potentialMate.TryGetComponent(out AnimalController potentialMateAnimalController) && potentialMateAnimalController.animalModel.IsAlive && !potentialMateAnimalController.animalModel.isPregnant && potentialMateAnimalController.animalModel.WantingOffspring 
                    && !(potentialMateAnimalController.fsm.currentState is MatingState) && !(potentialMateAnimalController.fsm.currentState is Waiting))
                {
                    
                    return potentialMateAnimalController.gameObject;
                }
            }

            return null;
        }
    }
}