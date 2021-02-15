using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;


//Author: Alexander LV
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/

//Also, Singleton pattern: https://csharpindepth.com/articles/singleton

//State where the animal searches actively for a mate.
//sealed just prevents other classes from inheriting. Not necessary for pattern since private constructor already makes sure of this.
//However might improve optimization.
public sealed class SearchForMate : FSMState<Animal>
{
    private static readonly SearchForMate instance = new SearchForMate();
    public static SearchForMate Instance {
        get {
            return instance;
        }
    }
    static SearchForMate() { }
   //Hide constructor
    private SearchForMate() { }

    public override int getStateID()
    {
        return 1;
    }

    public override void Enter (Animal a) {
        Debug.Log("Time to find me a mate");
        a.nav.enabled = true;
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
