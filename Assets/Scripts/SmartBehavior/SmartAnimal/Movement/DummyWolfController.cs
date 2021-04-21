using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using UnityEngine;

public class DummyWolfController : AnimalController
{
    new void Awake()
    {
        base.Awake();
        animalModel = new WolfModel();
        animalModel.reproductiveUrge = 100f;
        fsm.SetDefaultState(idleState);
        fsm.ChangeState(idleState);
        fsm.currentState.currentStateAnimation = StateAnimation.Idle;
        
    }
    public override void ChangeModifiers(State state)
    {
        //Do nothing
    }

    public override void UpdateParameters()
    {
       //do nothing
    }

    public override Vector3 getNormalizedScale()
    {
        return new Vector3(0.25f, 0.5f, 0.25f);
    }
    
    public override string GetObjectLabel()
    {
        return "DummyWolf";
    }
}
