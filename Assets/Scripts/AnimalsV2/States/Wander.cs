/*
 * Author: Johan A.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class Wander : State 
    {
        private List<String> priorities = new List<String>();

        private GameObject food;
        private GameObject water;
        private GameObject mate;

        public GameObject target;
        
        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Running;
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            var position1 = animal.transform.position;
            food = NavigationUtilities.GetNearestObjectPosition(animal.visibleFoodTargets, position1);
            water = NavigationUtilities.GetNearestObjectPosition(animal.visibleWaterTargets, position1);
            mate = NavigationUtilities.GetNearestObjectPosition(animal.visibleFriendlyTargets, position1);
            
            if (animal.agent.remainingDistance < 1.0f)
            {
                Vector3 position = new Vector3(Random.Range(-30.0f, 30.0f), 0, Random.Range(-30.0f, 30.0f));
                animal.agent.SetDestination(position);
            }
        }
        
        public GameObject FoundObject()
        {
            foreach (var p in priorities)
            {
                switch (p)
                {
                    case "Food":
                        if (food != null) return food;
                        break;
                    case "Water":
                        if (water != null) return water;
                        break;
                    case "Mate":
                        if (mate != null) return mate;
                        break;
                }
            }

            return null;
        }

        public void SetPriorities(List<String> priorities)
        {
            this.priorities = priorities;
        }
    }
}