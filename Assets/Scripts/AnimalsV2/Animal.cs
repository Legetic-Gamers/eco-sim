using System;
using FSM;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2.States;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


//Author: Alexander LV, Johan A
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
// Used Unity Official Tutorial on the Animator
public class Animal : MonoBehaviour
{

    //public event Action<FSMState<Animal>> OnStateChanged;

    NavMeshAgent agent;
    
    public StateMachine Fsm;
    public SearchForMate sm;
    public SearchingForFood sf;
    public SearchingForWater sw;
    public Idle idle;
    
    //Perceptions
    //Some form of hearing
    //Some form of Smell
    //Some form of sight

    //Parameters of the animal
    public float Hunger = 0;
    public int Energy = 0;
    public int Thirst = 0;
    public int ReproductiveUrge = 0;

    public void Eat(int amount)
    {
        //Update hunger, not below 0.
        Hunger = Math.Max(Hunger - amount,0);
    }

    //Taken from:  https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
    //returns true if animal is thirsty.
    public bool Thirsty() {
        bool thirsty = Thirst >= 10 ? true : false;
        return thirsty;
    }

    public bool Hungry() {
        bool hungry = Hunger >= 2 ? true : false;
        return hungry;
    }
    
    void Awake(){
        Debug.Log("Rabbit exists");

        // Get the NavMesh agent
        agent = this.GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }

    private void Start()
    {
        Fsm = new StateMachine();

        sf = new SearchingForFood(this, Fsm);
        sw = new SearchingForWater(this, Fsm);
        sm = new SearchForMate(this, Fsm);
        idle = new Idle(this, Fsm);

        Fsm.Initialize(idle);
    }

    // Update is called once per frame
    void Update()
    {
        //Tick parameters
        Hunger += 1 * Time.deltaTime;
        Thirst++;
        Fsm.CurrentState.HandleInput();

        Fsm.CurrentState.LogicUpdate();
        
        if (agent.remainingDistance < 1.0f){
            agent.destination = Random.insideUnitCircle * 20;
        }
    }
    
    private void FixedUpdate()
    {
        Fsm.CurrentState.PhysicsUpdate();
    }
}
