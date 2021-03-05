/*
 * Author: Johan A.
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AnimalsV2.Priorities;
using Random = UnityEngine.Random;

namespace AnimalsV2.States
{
//State where the animal just sits/ stands still.
//sealed just prevents other classes from inheriting
    public class Wander : State 
    {
        //private List<Priorities> priorities = new List<Priorities>();

        private GameObject food;
        private GameObject water;
        private GameObject mate;

        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            /*
            var position1 = animal.transform.position;
            food = NavigationUtilities.GetNearestObjectPosition(animal.visibleFoodTargets, position1);
            
            water = NavigationUtilities.GetNearestObjectPosition(animal.visibleWaterTargets, position1);
            mate = NavigationUtilities.GetNearestObjectPosition(animal.visibleFriendlyTargets, position1);
            */
            if (animal.agent.isActiveAndEnabled)
            {
                if (animal.agent.remainingDistance < 1.0f)
                {
                    Vector3 position = new Vector3(Random.Range(-30.0f, 30.0f), 0, Random.Range(-30.0f, 30.0f));
                    animal.agent.SetDestination(position);
                }

            }
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
        {
            Vector3 randDirection = Random.insideUnitSphere * dist;

            randDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

            return navHit.position;
        }
/*
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
        */
        public override string ToString()
        {
            return "Wandering";
        }

        public override bool MeetRequirements()
        {
            return true;
        }
    }
}