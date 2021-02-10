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
  public void  Update() {
    if (GlobalState != null)  GlobalState.Execute(Owner);
    if (CurrentState != null) CurrentState.Execute(Owner);
  }

        //Change current state.
  public void  ChangeState(FSMState<T> NewState) {
    PreviousState = CurrentState;
    if (CurrentState != null)
      CurrentState.Exit(Owner);
      CurrentState = NewState;
      if (CurrentState != null)
        CurrentState.Enter(Owner);
  }

  public void  RevertToPreviousState() {
    if (PreviousState != null)
      ChangeState(PreviousState);
  }
};

}