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
            stateAnimation = StateAnimation.Walking;
        }

        public override void Enter()
        {
            base.Enter();

            nextPosition = animal.transform.position;

            //Make an update instantly
            LogicUpdate();
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

            if (!animal.agent.isOnNavMesh)
            {
                //if agents is not placed on navmesh, warp that bad boy
                NavMeshHit hit;
                if (NavMesh.SamplePosition(animal.transform.position, out hit, 1000f, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    animal.agent.Warp(hit.position);    
                }
                else
                {
                    Debug.LogError("Agent is not on navmesh and can not be warped");
                }
                
            }
            
            if (animal.agent != null && animal.agent.isActiveAndEnabled)
            {
                if (Vector3.Distance(animal.transform.position, nextPosition) <= animal.agent.stoppingDistance + 0.2 || animal.agent.velocity.magnitude <= 0.1f)
                {
                    //Debug.Log(animal.agent.velocity.magnitude);
                    //Vector3 position = new Vector3(Random.Range(-10.0f, 10.0f), 0, Random.Range(-10.0f, 10.0f)) + animal.transform.position;
                    //Debug.Log("Framme!");
                    //Move the animal using the navmeshagent.
                    //TODO this maxDistance is what is causing rabbits to dance sometimes, if poisition cant be found.
                    // ALEXANDER H: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html recommends setting maxDistance as agents height * 2

                    float animalScale = Mathf.Max(1, animal.agent.height);
                    if(NavigationUtilities.RandomPoint(animal.transform.position, 8 * animalScale,10f * animalScale, out nextPosition))
                    {
                        //animal.agent.SetDestination(nextPosition);
                        
                        //To avoid async path calculation we do this
                        NavMeshPath path = new NavMeshPath();
                        animal.agent.CalculatePath(nextPosition, path);
                        if (path.status != NavMeshPathStatus.PathInvalid)
                        {
                            animal.agent.SetPath(path);
                        }
                    }else
                    {
                        Debug.Log("Agent stuck, Dist: " + Vector3.Distance(animal.transform.position, nextPosition)+ " Stopping: " + animal.agent.stoppingDistance + 0.2);
                    }
                }
            }
        }
        
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