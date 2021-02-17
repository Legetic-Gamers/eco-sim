using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using static FSM.StateAnimation;

public class SearchForFood : FSMState<Animal>
{
    static readonly SearchForFood instance = new SearchForFood();
    public static SearchForFood Instance {
        get {
            return instance;
        }
    }
    static SearchForFood() { }
    private SearchForFood() { }
    

    public override void Enter (Animal a) {
        Debug.Log("Search for food...");
        currentStateAnimation = LookingOut;
    }

    public override void Execute (Animal a)
    {
        currentStateAnimation = Walking;
        
    }

    public override void Exit(Animal a) {

    }
}