namespace AnimalsV2.States
{
    using System;
    using UnityEngine;

    namespace AnimalsV2.States
    {
        public class Drinking : State
        {
            public Action<GameObject> onDrinkWater;
        
            public Drinking(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
            {
                
            }

            public override void Enter()
            {
                base.Enter();
                currentStateAnimation = StateAnimation.Idle;
            }

            public override void HandleInput()
            {
                base.HandleInput();
            
            }

            public override void LogicUpdate()
            {
                base.LogicUpdate();
             
            }

            public void DrinkWater(GameObject target)
            {
                onDrinkWater?.Invoke(target);
            }

            public override string ToString()
            {
                return "Drinking";
            }
        }
    }
}