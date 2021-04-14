using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using UnityEngine;


public class AnimalPlaySound : Common_PlaySound
{
    private AnimalController animalController;
    private FiniteStateMachine fsm;
    
    void Start()
    {
        animalController = GetComponent<AnimalController>();
        fsm = animalController.fsm;
    
        EventSubscribe();
    }
    
    private void EventSubscribe()
    {
        animalController.fsm.OnStateEnter += MakeStateSound;
        
    }
    private void EventUnsubscribe()
    {
        if(animalController != null) animalController.fsm.OnStateEnter -= MakeStateSound;
    }
    
    private void MakeStateSound(State state)
    {
        switch (state)
        {
            case Wander _ :
                Walking();
                break;
            case EatingState _ :
                Eating();
                break;
            case FleeingState _ :
                Running();
                break;
            case Dead _ :
                Death();
                break;
        }
    }
    
    private void OnDestroy()
    {
        EventUnsubscribe();
    }
}