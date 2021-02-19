using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using UnityEngine.AI;

//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class SearchForFood : State
    {
        private Vector3 foodPos;
        public SearchForFood(Animal animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
        }

        public void Enter()
        {
            base.Enter();
            Debug.Log("ENTERED SEARCHING FOR FOOD");
            currentStateAnimation = StateAnimation.Running;
        }

        public void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of enemies and run away from it.
            foodPos = NavigationUtilities.GetNearestObjectPositionByTag(animal, "Food");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,foodPos,true);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }

        public bool adjacentToFood()
        {
            return Vector3.Distance(animal.transform.position, foodPos) < 10;
        }
    }
}