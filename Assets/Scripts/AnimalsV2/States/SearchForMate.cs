using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;


//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//State where the animal searches actively for a mate.
//sealed just prevents other classes from inheriting
public sealed class SearchForMate : FSMState<Animal>
{
    static readonly SearchForMate instance = new SearchForMate();
    public static SearchForMate Instance {
        get {
            return instance;
        }
    }
    static SearchForMate() { }
    private SearchForMate() { }

    public override void Enter (Animal a) {
        Debug.Log("Time to find me a mate");
        a.anim.SetBool("isJumping", true);
       /* if (m.Location != Locations.goldmine) {
            Debug.Log("Entering the mine...");
            m.ChangeLocation(Locations.goldmine);
        }*/
    }

    public override void Execute (Animal a) {
        //Searching behavior goes here

        /*
        m.AddToGoldCarried(1);
        Debug.Log("Picking ap nugget and that's..." +
                  m.GoldCarried);
        m.IncreaseFatigue();
        if (m.PocketsFull())
            m.ChangeState(VisitBankAndDepositGold.Instance);
        */
    }

    public override void Exit(Animal a) {
        
        Debug.Log("Wonder if i found a mate? Alright imma stop looking at least.");
        

    }
}
