using System;
using AnimalsV2;
using UnityEngine;

//Author: Alexander LV
// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//Represents a finite state where T is the type of the owner of the FSM.

namespace FSM
{
    public enum StateAnimation
    {
        Running,
        Walking, 
        LookingOut,
        Idle, 
        Dead
    }
    public abstract class State
    {
        protected StateAnimation currentStateAnimation = StateAnimation.Idle;
        
        protected Animal animal;
        protected StateMachine stateMachine;
        

        protected State(Animal animal, StateMachine stateMachine)
        {
            this.animal = animal;
            this.stateMachine = stateMachine;
        }
        
        //ENTER
        public virtual void Enter()
        {
            
        }

        //DURING UPDATE()
        public virtual void HandleInput()
        {
            
        }

        public virtual void LogicUpdate()
        {
            
        }
        
        
        //DURING FIXEDUPDATE
        public virtual void PhysicsUpdate()
        {
        }
        
        
        //EXIT
        public virtual void Exit()
        {
            
        }


        public override string ToString()
        {
            return currentStateAnimation.ToString();
        }
    }
}