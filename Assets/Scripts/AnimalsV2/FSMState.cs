using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Alexander LV
// Source: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//Represents a finite state where T is the type of the owner of the FSM.
namespace FSM
{

abstract public class FSMState <T>
{
    abstract public int getStateID();
    abstract public void Enter(T entity);
    abstract public void Execute (T entity);
    abstract public void Exit(T entity);

    //Override isEqual?
}

}