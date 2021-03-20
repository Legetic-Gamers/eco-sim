﻿using UnityEngine.AI;

namespace AnimalsV2.States
{
    using System;
using UnityEngine;

    public class GoToWater : State
    {

        public GoToWater(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
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
                GameObject closestWater = NavigationUtilities.GetNearestObject(animal.visibleWaterTargets, animal.transform.position);
                if (closestWater != null && animal.agent.isActiveAndEnabled)
                {
                    Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform, closestWater.transform.position, true);
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
            return "Go to water";
        }

        public override bool MeetRequirements()
        {
            // rewuirements for this state are following
            return animal.visibleWaterTargets.Count > 0;
        }
    }
}
