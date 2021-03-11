using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2.States;
using UnityEditorInternal;
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
        public State CurrentState { get; private set; }
        
        private State defaultState { get; set; }
        
        // Used to identify an absorbing state, such that no other state can be entered, e.g. Dead.
        public bool absorbingState;
        
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
            ChangeState(startingState);
        }

        /// <summary>
        /// Changing states. 
        /// </summary>
        /// <param name="newState"> State to change into. </param>
        public bool ChangeState(State newState)
        {
            // if (newState is MatingState)
            // {
            //     Debug.Log("absorbingstate:" + absorbingState + " Meetrequirements: " + newState.MeetRequirements());
            // }

            // if the state is absorbing, meaning that state change is not possible or newState == CurrentState or newState does not meet requirements, we return
            if(newState == CurrentState || absorbingState || !newState.MeetRequirements()) return false;
            
            if (CurrentState != null)
            {
                //Exit old state
                CurrentState.Exit();
                OnStateExit?.Invoke(CurrentState);
            }

            //Change state
            CurrentState = newState;
            if (CurrentState != null)
            {
                //Enter new state
                CurrentState.Enter();
                OnStateEnter?.Invoke(CurrentState);
                return true;
                
            }
            return false;
        }
        
        public void  UpdateStatesLogic() {
            if (CurrentState != null) CurrentState.LogicUpdate();
            //Debug.Log(OnStateLogicUpdate.ToString());
            OnStateLogicUpdate?.Invoke(CurrentState);
        }
        
        public virtual void HandleStatesInput()
        {
            if (CurrentState != null) CurrentState.HandleInput();
        }
        
        public virtual void UpdateStatesPhysics()
        {
            if (CurrentState != null) CurrentState.PhysicsUpdate();
            OnStatePhysicsUpdate?.Invoke(CurrentState);
        }

        public void GoToDefaultState()
        {
            ChangeState(defaultState);
        }
    }
}