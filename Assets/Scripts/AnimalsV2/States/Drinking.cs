namespace AnimalsV2.States
{
    using System;
    using UnityEngine;

    namespace AnimalsV2.States
    {
        public class Drinking : State
        {
            private float timeLeft;
            private bool doneDrinking;
            
            public Action<GameObject> onDrinkWater;
        
            public Drinking(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
            {
                timeLeft = 3.0f;
                doneDrinking = false;
            }

            public override void Enter()
            {
                base.Enter();
                //Debug.Log("Eating!");
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