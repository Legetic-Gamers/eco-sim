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

    // private void Awake()
    // {
    //     animalController = GetComponent<AnimalController>();
    //     
    // }
    //
    // void Start()
    // {
    //     fsm = animalController.fsm;
    //
    //     EventSubscribe();
    // }
    //
    // private void EventSubscribe()
    // {
    //     animalController.fsm.OnStateEnter += MakeStateSound;
    //     
    // }
    // private void EventUnsubscribe()
    // {
    //     animalController.fsm.OnStateEnter -= MakeStateSound;
    //    
    // }
    
    private void MakeStateSound(State state)
    {

        // if (state is Mating)
        // {
        //     AnimalSound();
        // }
        // else if (state is GoToState)
        // {
        //     Walking();
        //     
        // }else if (state is Eating)
        // {
        //     Eating();
        // }
    }
    
    // private void OnDestroy()
    // {
    //     EventUnsubscribe();
    // }
}