/*
 * Authors: Johan A, Alexander L.V.
 */
using System;
using FSM;

namespace AnimalsV2
{
    /// <summary>
    /// Abstract class that all states inherits. StateAnimation is used in AnimationController to animate. 
    /// </summary>
    //TODO Change System with animationcontroller. 
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
        /// <summary>
        /// Sates have an owner (Animal) and a stateMachine to control them. 
        /// </summary>
        
        protected StateAnimation currentStateAnimation = StateAnimation.Idle;
        protected Animal animal;
        protected StateMachine stateMachine;
        
        // Actions for animator. 
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

        public virtual void HandleInput() {}

        public virtual void LogicUpdate()
        {
            OnStateExecute?.Invoke(stateMachine.CurrentState);
        }

        public virtual void PhysicsUpdate() {}

        public virtual void Exit()
        {
            OnStateExit?.Invoke(stateMachine.CurrentState);
        }
    }
}