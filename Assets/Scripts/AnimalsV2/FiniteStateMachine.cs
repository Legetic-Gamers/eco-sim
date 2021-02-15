using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Author: Alexander LV
// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

namespace FSM
 {


//The finite state machine, which handles the current state and switching between states.
public class FiniteStateMachine <T>  {
  private T Owner;
  private FSMState<T> CurrentState;
  private FSMState<T> PreviousState;
  private FSMState<T> GlobalState;


  //State Changed Listeners. Used so that class is decoupled from view (animation).
  //Passes in the state that was changed to.
  public event Action<FSMState<T>> OnStateEnter;
  public event Action<FSMState<T>> OnStateExecute;
  public event Action<FSMState<T>> OnStateExit;

  public void Awake() {
    CurrentState = null;
    PreviousState = null;
    GlobalState = null;
  }

        //Initialization method which sets the owner and inital state.
  public void Configure(T owner, FSMState<T> InitialState) {
    Owner = owner;
    ChangeState(InitialState);
  }
        //Update the state 
  public void  UpdateState() {
    if (GlobalState != null)  GlobalState.Execute(Owner);
    if (CurrentState != null) CurrentState.Execute(Owner);

    //Notify execute state listeners.
    
    OnStateExecute?.Invoke(CurrentState);
        //Debug.Log("EXECTUUTUEUTD");

    


  }

        //Change current state.
  public void  ChangeState(FSMState<T> NewState) {
      
      if(NewState == CurrentState)return;
      
    PreviousState = CurrentState;
    if (CurrentState != null)
    {
        
        

        //Exit state and change state
        CurrentState.Exit(Owner);
        //Notify exit state listeners, animationcontroller for example.
        OnStateExit?.Invoke(CurrentState);

    }
        CurrentState = NewState; 

        if (CurrentState != null)
        {
            
            CurrentState.Enter(Owner);
            //Notify Enter State listeners and enter new state
            OnStateEnter?.Invoke(CurrentState);
        }
    
  }

  public void  RevertToPreviousState() {
    if (PreviousState != null)
      ChangeState(PreviousState);
  }
};

}