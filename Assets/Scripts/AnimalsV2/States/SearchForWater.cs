using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using static FSM.StateAnimation;

public class SearchForWater : FSMState<Animal>
{
    static readonly SearchForWater instance = new SearchForWater();
    public static SearchForWater Instance {
        get {
            return instance;
        }
    }
    static SearchForWater() { }
    private SearchForWater() { }
    

    public override void Enter (Animal a) {
        Debug.Log("Searching for water...");
        currentStateAnimation = LookingOut;
    }

    public override void Execute (Animal a) {
        
    }

    public override void Exit(Animal a) {

    }
}