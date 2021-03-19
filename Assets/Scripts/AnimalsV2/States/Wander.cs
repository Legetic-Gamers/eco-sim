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

        private Vector3 nextPosition;

        public Wander(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            currentStateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();
            
            nextPosition = NavigationUtilities.RandomNavSphere(animal.transform.position, 10,
                1 << NavMesh.GetAreaFromName("Walkable"));
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
                if (animal.agent.remainingDistance <= animal.agent.stoppingDistance)
                {
                    //Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f)) + animal.transform.position;
                   
                    //Move the animal using the navmeshagent.
                    NavMeshHit hit;
                    //TODO this maxDistance is what is causing rabbits to dance sometimes, if poisition cant be found.
                    // ALEXANDER H: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html recommends setting maxDistance as agents height * 2
                    // if (NavMesh.SamplePosition(nextPosition, out hit, animal.agent.height * 2, 1 << NavMesh.GetAreaFromName("Walkable")))
                    // {
                    //     animal.agent.SetDestination(hit.position);
                    //     
                    //     //Set next 
                    //     nextPosition = NavigationUtilities.RandomNavSphere(animal.transform.position, 10,
                    //         1 << NavMesh.GetAreaFromName("Walkable"));
                    // }
                    // else
                    // {
                    //     nextPosition = new Vector3(-nextPosition.x, nextPosition.y, -nextPosition.z);
                    // }

                    if(RandomPoint(animal.transform.position, 10f, out nextPosition))
                    {
                        animal.agent.SetDestination(nextPosition);
                    }


                }

            }
        }
        
        //https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, animal.agent.height * 2, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                    
                    //Try opposite direction to avoid clinging to walls.
                }else if (NavMesh.SamplePosition(new Vector3(-randomPoint.x, randomPoint.y, -randomPoint.z), out hit,
                    animal.agent.height * 2, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
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