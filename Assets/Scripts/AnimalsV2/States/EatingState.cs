using System;
using UnityEngine;

namespace AnimalsV2.States
{
    public class EatingState : State
    {

        public Action<GameObject> onEatFood;

        private GameObject target;
        
        public EatingState(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Attack;
            GetNearestFood();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (MeetRequirements())
            {
                EatFood(target);
            }
            else
            {
                finiteStateMachine.GoToDefaultState();
            }
        }
        

        public void EatFood(GameObject target)
        {
            onEatFood?.Invoke(target);
            finiteStateMachine.GoToDefaultState();
        }

        public override string ToString()
        {
            return "Eating";
        }

        public override bool MeetRequirements()
        {
            Vector3 position = animal.transform.position;
            target = GetNearestFood();
            if (target == null) return false;
            bool isCloseEnough = Vector3.Distance(target.transform.position, position) <= 2f;
            return animal.visibleFoodTargets.Count > 0 && isCloseEnough;
        }

        private GameObject GetNearestFood()
        {
            Vector3 position = animal.transform.position;
            return NavigationUtilities.GetNearestObjectPosition(animal.visibleFoodTargets, position);
        }
        
        
    }
}