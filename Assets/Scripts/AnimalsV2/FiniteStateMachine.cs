﻿using System;
using AnimalsV2.States;
using UnityEngine;


namespace AnimalsV2
 {
     /// <summary>
     /// State Machine handles States.
     /// Author: Alexander LV, Johan A
     /// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
     /// </summary>
     public class FiniteStateMachine
    {
        public State currentState { get; set; }
        
        private State defaultState { get; set; }
        
        // Used to identify an absorbing state, such that no other state can be entered, e.g. Dead.
        public bool isLocked;
        
        //State Change Listeners
        public event Action<State> OnStateEnter;
        public event Action<State> OnStateLogicUpdate;
        public event Action<State> OnStatePhysicsUpdate;
        public event Action<State> OnStateExit;

        /// <summary>
        /// Start the state machine in a non-empty state
        /// </summary>
        /// <param name="startingState"> State to start in (Idle) </param>
        public void Initialize(State startingState)
        {
            defaultState = startingState;
            ForceDefaultState();
        }

        /// <summary>
        /// Changing states. 
        /// </summary>
        /// <param name="newState"> State to change into. </param>
        public bool ChangeState(State newState, bool force = false)
        {
            if (force)
            {
                isLocked = false;
            }

            // if the state is absorbing, meaning that state change is not possible or newState == CurrentState or newState does not meet requirements, we return
            if(isLocked || !newState.MeetRequirements()) return false;
            //If we try to enter same state, don't do anything but essentially the state change was good.
            if (newState == currentState) return true;

            if (currentState != null)
            {
                //Exit old state
                currentState.Exit();
                OnStateExit?.Invoke(currentState);
            }

            //Change state
            currentState = newState;
            if (currentState != null)
            {
                //Enter new state
                currentState.Enter();
                OnStateEnter?.Invoke(currentState);
                return true;
                
            }
            return false;
        }
        
        public void  UpdateStatesLogic() {
            if (currentState != null) currentState.LogicUpdate();
            //Debug.Log(OnStateLogicUpdate.ToString());
            OnStateLogicUpdate?.Invoke(currentState);
        }
        
        public virtual void HandleStatesInput()
        {
            if (currentState != null) currentState.HandleInput();
        }
        
        public virtual void UpdateStatesPhysics()
        {
            if (currentState != null) currentState.PhysicsUpdate();
            OnStatePhysicsUpdate?.Invoke(currentState);
        }

        public bool GoToDefaultState()
        {
            return ChangeState(defaultState);
        }

        public void ForceDefaultState()
        {
            ChangeState(defaultState, true);
        }

        public void SetDefaultState(State state)
        {
            defaultState = state;
        }
        
    }
}