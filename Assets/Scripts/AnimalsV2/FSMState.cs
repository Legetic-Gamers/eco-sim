

//Author: Alexander LV
// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//Represents a finite state where T is the type of the owner of the FSM.

using static FSM.StateAnimation;

namespace FSM
{
    
    //TODO Could potentially just get all possible animations.
    public enum StateAnimation
    {
        Walking,
        Running,
        LookingOut,
        JumpingUp,
        Idle,
        Dead
    }
    abstract public class FSMState<T>
    {
        //protected = seen by children.
        protected StateAnimation currentStateAnimation = StateAnimation.Idle;

         abstract public void Enter(T entity);
        abstract public void Execute(T entity);
        abstract public void Exit(T entity);

        //You can override GetStateAnimation if custom evaluation is wanted.
        
        public override string ToString()
        {
            return currentStateAnimation.ToString();
        }


        //Override isEqual?
    }
}