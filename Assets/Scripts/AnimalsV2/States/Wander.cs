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
                    Vector3 position = NavigationUtilities.RandomNavSphere(animal.transform.position, 10,
                        1 << NavMesh.GetAreaFromName("Walkable"));
                    
                    //Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f)) + animal.transform.position;
                   
                    //Move the animal using the navmeshagent.
                    NavMeshHit hit;
                    //TODO this maxDistance is what is causing rabbits to dance sometimes, if poisition cant be found.
                    // ALEXANDER H: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html recommends setting maxDistance as agents height * 2
                    if (NavMesh.SamplePosition(position, out hit, animal.agent.height * 2, 1 << NavMesh.GetAreaFromName("Walkable")))
                    {
                        animal.agent.SetDestination(hit.position);
                    }
                    

                }

            }
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