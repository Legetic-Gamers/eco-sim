/*
 * Authors: Johan A, Alexander L.V.
 */

using System;

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
        Attack,
        Idle,
        Mating,
        Dead
    }

    public enum Priorities
    {
        Food,
        Water,
        Mate
    }

    public abstract class State
    {
        /// <summary>
        /// Sates have an owner (Animal) and a stateMachine to control them. 
        /// </summary>
        protected StateAnimation currentStateAnimation = StateAnimation.Idle;

        protected AnimalController animal;
        protected FiniteStateMachine finiteStateMachine;


        protected State(AnimalController animal, FiniteStateMachine finiteStateMachine)
        {
            this.animal = animal;
            this.finiteStateMachine = finiteStateMachine;
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
        
        public string GetStateAnimation()
        {
            return currentStateAnimation.ToString();
        }

        // used to display state in the UI
        public abstract string ToString();

        public abstract bool MeetRequirements();
        
    }
}