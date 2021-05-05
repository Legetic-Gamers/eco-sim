using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AnimalsV2.States
{
    public class Wander2 : State
    {
        private WanderSubState currentWanderSubstate;
        private int ticksLeftOfWanderSubstate;
        private Vector3 movementVector;
        
        public Wander2(AnimalController animal, FiniteStateMachine finiteStateMachine) : base(animal, finiteStateMachine)
        {
            stateAnimation = StateAnimation.Walking;
            movementVector = Vector3.zero;
        }

        public override string ToString()
        {
            return currentWanderSubstate.ToString();
        }

        public override bool MeetRequirements()
        {
            return true;
        }

        public override void Enter()
        {
            base.Enter();
            movementVector = Vector3.zero;
            ticksLeftOfWanderSubstate = 0;
            LogicUpdate();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            
            if (!animal.agent.isOnNavMesh)
            {
                //if agents is not placed on navmesh, warp that bad boy
                NavMeshHit hit;
                if (NavMesh.SamplePosition(animal.transform.position, out hit, 10f, 1 << NavMesh.GetAreaFromName("Walkable")))
                {
                    bool succesfulWarp = animal.agent.Warp(hit.position);

                    if (!succesfulWarp)
                    {
                        Debug.LogError("Agent is not on navmesh and can not be warped");
                    }  
                }
            }

            if (animal.agent != null && animal.agent.isActiveAndEnabled)
            {
                //decide which substate to enter
                if (ticksLeftOfWanderSubstate == 0)
                {
                    float rand = Random.value;

                    if (rand > 0.95)
                    {
                        //long walk init
                        LongWalkInit();
                    } else if (rand > 0.4)
                    {
                        //short walk init
                        ShortWalkInit();
                    }
                    else
                    {
                        //look around init
                        LookAroundInit();
                    }
                }


                if (animal.agent != null && animal.agent.isActiveAndEnabled)
                {
                    switch (currentWanderSubstate)
                    {
                        case WanderSubState.LongWalk:
                            WalkUpdate(10);
                            break;
                        case WanderSubState.ShortWalk:
                            WalkUpdate(20);
                            break;
                        case WanderSubState.LookAround:
                            LookAroundUpdate();
                            break;
                    }
                }
                
            }
            else
            {
                Debug.Log("Agent is null or inactive or enabled or outside of navmesh");
            }
            
        }

        void LongWalkInit()
        {

            if (currentWanderSubstate != WanderSubState.LongWalk)
            {
                animal.animationController.CrossOverAnimation(StateAnimation.Walking.ToString());
            }
            currentWanderSubstate = WanderSubState.LongWalk;                


            //movement vector is set as local vector
            movementVector = animal.transform.forward;
            
            ticksLeftOfWanderSubstate = 14;
        }

        void ShortWalkInit()
        {
            if (currentWanderSubstate != WanderSubState.ShortWalk)
            {
                animal.animationController.CrossOverAnimation(StateAnimation.Walking.ToString());
            }
            currentWanderSubstate = WanderSubState.ShortWalk;                


            //movement vector is set as local vector
            movementVector = Random.insideUnitSphere;
            movementVector.y = 0;
            movementVector = movementVector.normalized;
            
            
            ticksLeftOfWanderSubstate = 5;
        }

        void LookAroundInit()
        {
            
            animal.agent.ResetPath();
            if (currentWanderSubstate != WanderSubState.LookAround)
            {
                animal.animationController.CrossOverAnimation(StateAnimation.Idle.ToString());
            }
            currentWanderSubstate = WanderSubState.LookAround;

            ticksLeftOfWanderSubstate = 2;
        }
        

        void WalkUpdate(float maxAngle)
        {
            Vector3 tempVector = movementVector * 5;
            
            float rotationPercentage = Random.Range(-1, 1);  //sample a number between -1,1 which is percentage of maxAngle to rotate
            float rotationAngle = maxAngle * rotationPercentage;    //get the rotation in degrees
            tempVector = Quaternion.AngleAxis(rotationAngle, Vector3.up) * tempVector;  //rotate forward vector
            tempVector += animal.transform.position; //add position since we navigate with global coordinates
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(tempVector, out hit, animal.agent.height * 2,
                1 << NavMesh.GetAreaFromName("Walkable")))
            {
                NavigationUtilities.NavigateToPoint(animal, tempVector);
            } else if (NavigationUtilities.PerpendicularPoint(animal.transform.position,animal.transform.forward,animal.transform.up,animal.agent.height*2 + 2f,out movementVector))
            {
                //Debug.Log("PERPENDICULAR");
                NavigationUtilities.NavigateToPoint(animal, movementVector);
                movementVector = animal.transform.forward;
            } else if (NavigationUtilities.RandomPoint(animal.transform.position, 10, 10f, out movementVector))
            {
                NavigationUtilities.NavigateToPoint(animal, movementVector);
                movementVector = animal.transform.forward;
            }    
            
            DecrementTick();
        }

        void LookAroundUpdate()
        {
            DecrementTick();
        }

        void DecrementTick()
        {
            if (ticksLeftOfWanderSubstate > 0)
            {
                ticksLeftOfWanderSubstate--;
            }
        }
        
        private enum WanderSubState
        {
            LongWalk, ShortWalk, LookAround
        }
    }
}