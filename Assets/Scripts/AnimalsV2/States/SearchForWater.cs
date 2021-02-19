using System.Numerics;
using FSM;
using UnityEngine;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class SearchForWater : State
    {

        public SearchForWater(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("Searching for water!");
            currentStateAnimation = StateAnimation.LookingOut;

        }

        public override void HandleInput()
        {
            base.HandleInput();

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

        }
        
        public bool adjacentToWater()
        {
            //return Vector3.Distance(animal.transform.position, foodPos) < 10;
            return false;
        }
    }
}