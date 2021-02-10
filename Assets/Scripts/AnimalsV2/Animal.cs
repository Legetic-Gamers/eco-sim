using System;
using FSM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Author: Alexander LV, Johan A
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
// Used Unity Official Tutorial on the Animator
public class Animal : MonoBehaviour
{

    private FiniteStateMachine<Animal> FSM;
    public Animator anim;

    //Perceptions
    //Some form of hearing
    //Some form of Smell
    //Some form of sight

    //Parameters of the animal
    public int Hunger = 0;
    public int Energy = 0;
    public int Thirst = 0;
    public int ReproductiveUrge = 0;

    void Awake(){

        Debug.Log("Rabbit exists");
        FSM = new FiniteStateMachine<Animal>();

        //For now get instance of the state. Could also be switched to instance-based.
        FSM.Configure(this, Idle.Instance);
     }

    public void ChangeState(FSMState<Animal> state)
    {
        FSM.ChangeState(state);
    }

    public void Eat(int amount)
    {
        //Update hunger, not below 0.
        Hunger = Math.Max(Hunger - amount,0);

    }

    //Taken from:  https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
    //returns true if animal is thirsty.
    public bool Thirsty() {
        bool thirsty = Thirst == 10 ? true : false;
        return thirsty;
    }

    public bool Hungry() {
        bool hungry = Hunger == 10 ? true : false;
        return hungry;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Tick parameters
        Hunger++;
        Thirst++;
        if(Hungry()) ChangeState(SearchForMate.Instance);
        FSM.Update();
    }
}
