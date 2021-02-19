using FSM;
using UnityEngine;


//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class SearchForMate : State
    {

        public SearchForMate(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public void Enter()
        {
            base.Enter();
            Debug.Log("Searching for mate!");
            currentStateAnimation = StateAnimation.LookingOut;
        }

        public void HandleInput()
        {
            base.HandleInput();
            
        }

        public void LogicUpdate()
        {
            base.LogicUpdate();
            
        }
        
        public bool adjacentToMate()
        {
            //return Vector3.Distance(animal.transform.position, foodPos) < 10;
            return false;
        }
    }
}