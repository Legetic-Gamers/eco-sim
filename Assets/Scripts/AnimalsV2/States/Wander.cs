/*
 * Author: Johan A.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AnimalsV2.Priorities;
using Random = UnityEngine.Random;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class Wander : State 
    {
        private List<Priorities> priorities = new List<Priorities>();

        private GameObject food;
        private GameObject water;
        private GameObject mate;
        
        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine) {}

        public override void Enter()
        {
            base.Enter();
            currentStateAnimation = StateAnimation.Walking;
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
            if (mate != null)
            {
                //Friendlies are only potential mates if this and target also want offspring.
                if (!mate.GetComponent<AnimalController>().animalModel.WantingOffspring || !animal.animalModel.WantingOffspring)
                {
                    mate = null;
                }

            }

            if (animal.agent.isActiveAndEnabled)
            {
                if (animal.agent.remainingDistance < 1.0f)
                {
                    Vector3 position = new Vector3(Random.Range(-30.0f, 30.0f), 0, Random.Range(-30.0f, 30.0f));
                    animal.agent.SetDestination(position);
                }

            }
        }
        
        public Tuple<GameObject, Priorities> FoundObject()
        {
            foreach (var priority in priorities)
            {
                switch (priority)
                {
                    case Food:
                        if (food != null) return Tuple.Create(food, Food);
                        break;
                    case Water:
                        if (water != null) return Tuple.Create(water, Water);
                        break;
                    case Mate:
                        if (mate != null) return Tuple.Create(mate, Mate);
                        break;
                }
            }

            return null;
        }

        public void SetPriorities(List<Priorities> priorities)
        {
            this.priorities = priorities;
        }
        
        public override string ToString()
        {
            return "Wandering";
        }
    }
}