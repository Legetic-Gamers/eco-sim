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
        
        public event Action<State> OnStateEnter;
        public event Action<State> OnStateExecute;
        public event Action<State> OnStateExit;

        protected State(Animal animal, StateMachine stateMachine)
        {
            this.animal = animal;
            this.stateMachine = stateMachine;
        }
        public virtual void Enter()
        {
            OnStateEnter?.Invoke(stateMachine.CurrentState);
        }

        public virtual void HandleInput()
        {
            
        }

        public virtual void LogicUpdate()
        {
            OnStateExecute?.Invoke(stateMachine.CurrentState);
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void Exit()
        {
            OnStateExit?.Invoke(stateMachine.CurrentState);
        }
    }
}