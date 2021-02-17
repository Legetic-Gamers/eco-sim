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

        public SearchForFood(Animal animal, StateMachine stateMachine) : base(animal, stateMachine)
        {
        }

        public void Enter()
        {
            base.Enter();
            Debug.Log("ENTERED SEARCHING FOR FOOD");
        }

        public void HandleInput()
        {
            base.HandleInput();
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            //Get average position of enemies and run away from it.
            Vector3 foodPos = NavigationUtilities.GetNearestObjectByTag(animal, "Food");
            Vector3 pointToRunTo = NavigationUtilities.RunToFromPoint(animal.transform,foodPos,true);
            //Move the animal using the navmeshagent.
            NavMeshHit hit;
            NavMesh.SamplePosition(pointToRunTo,out hit,5,1 << NavMesh.GetAreaFromName("Walkable"));
            animal.agent.SetDestination(hit.position);
        }
    }
}