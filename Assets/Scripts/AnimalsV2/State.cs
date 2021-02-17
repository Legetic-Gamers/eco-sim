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

        //EXIt
        public virtual void Exit()
        {
        }
        
        public override string ToString()
        {
            return currentStateAnimation.ToString();
        }
    }
}