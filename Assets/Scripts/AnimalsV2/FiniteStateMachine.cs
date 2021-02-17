using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;




namespace AnimalsV2
 {
     /// <summary>
     /// State Machine handles States.
     /// Author: Alexander LV, Johan A
     /// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
     /// </summary>
     public class StateMachine
    {
        public State CurrentState { get; private set; }
        
        
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
            ChangeState(startingState);
            
        }

        /// <summary>
        /// Changing states. 
        /// </summary>
        /// <param name="newState"> State to change into. </param>
        public void ChangeState(State newState)
        {
            if(newState == CurrentState) return;
            
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
                Debug.Log("Entering state!");
                
                OnStateEnter?.Invoke(CurrentState);
            }
            
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

      

        
    }
}