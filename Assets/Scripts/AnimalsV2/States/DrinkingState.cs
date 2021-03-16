
    /*
 * Authors: Alexander L.V, Johan A.
 */

using System;
using UnityEngine;
using UnityEngine.AI;

namespace AnimalsV2.States
{
    public class DrinkingState : State
    {
        private GameObject target;

        public Func<GameObject,float> onDrinkWater;
        
        public DrinkingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Attack;
            GetNearestWater();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (MeetRequirements())
            {
                
                DrinkWater(target);
            }
            else
            {
                finiteStateMachine.GoToDefaultState();
            }
        }
        

        public void DrinkWater(GameObject target)
        {
            onDrinkWater?.Invoke(target);
            finiteStateMachine.GoToDefaultState();
        }

        public override string ToString()
        {
            return "Drinking";
        }

        public override bool MeetRequirements()
        {
            Vector3 position = animal.transform.position;
            target = GetNearestWater();
            if (target == null) return false;
            bool isCloseEnough = Vector3.Distance(target.transform.position, position) <= 3f;
            return animal.visibleWaterTargets.Count > 0 && isCloseEnough;
        }

        private GameObject GetNearestWater()
        {
            Vector3 position = animal.transform.position;
            return NavigationUtilities.GetNearestObject(animal.visibleWaterTargets, position);
        }
    }

}