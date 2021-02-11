using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
public class Eating : FSMState<Animal>
{
    static readonly Eating instance = new Eating();
    public static Eating Instance {
        get {
            return instance;
        }
    }
    static Eating() { }
    private Eating() { }

    public override int getStateID()
    {
        return 0;
    }

    public override void Enter (Animal a) {
        Debug.Log("Eating...");
        
    }

    public override void Execute (Animal a) {
        
    }

    public override void Exit(Animal a) {

    }
}