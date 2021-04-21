using UnityEngine.AI;

namespace AnimalsV2.States
{
    using System;
using UnityEngine;

    public class GoToWater : State
    {

        public GoToWater(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            stateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
            
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
                GameObject closestWater = NavigationUtilities.GetNearestObject(animal.visibleWaterTargets, animal.transform.position);
                if (closestWater != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo = closestWater.transform.position;
                    //Move the animal using the navmeshagent.
                    NavigationUtilities.NavigateToPoint(animal,pointToRunTo);
                    Vector3 a = new Vector3(animal.transform.position.x, 0, animal.transform.position.z);
                    Vector3 b = new Vector3(closestWater.transform.position.x, 0, closestWater.transform.position.z);
                    if(Vector3.Distance(a, b) <= animal.agent.stoppingDistance + 1.25f){
                        animal.drinkingState.SetTarget(closestWater);
                        finiteStateMachine.ChangeState(animal.drinkingState);
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
            return "Go to water";
        }

        public override bool MeetRequirements()
        {
            // rewuirements for this state are following
            return animal.visibleWaterTargets.Count > 0 && !animal.animalModel.HighHydration;
        }
    }
}
