using System;
using AnimalsV2.States;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

//Author: Alexander LV, Johan A
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
// Used Unity Official Tutorial on the Animator
namespace AnimalsV2
{
    public class Animal : MonoBehaviour
    {
    
        public NavMeshAgent agent;
    
        public StateMachine Fsm;
        public SearchForMate sm;
        public SearchForFood sf;
        public SearchForWater sw;
        public FleeingState fs;
        public Idle idle;

        // TODO Senses
        public GameObject[] nearbyObjects;

        // Parameters
        public float Hunger = 0;
        public int Energy = 0;
        public int Thirst = 0;
        public int ReproductiveUrge = 0;
        
    
        void Awake(){
            Debug.Log("Rabbit exists");

            // Get the NavMesh agent
            agent = GetComponent<NavMeshAgent>();
            agent.autoBraking = false;

            Fsm = new StateMachine();
            // AnimationController animationController = new AnimationController(this);
            
        }

        private void Start()
        {
            
            

            // sf = new SearchForFood(this, Fsm);
            // sw = new SearchForWater(this, Fsm);
            // sm = new SearchForMate(this, Fsm);
             fs = new FleeingState(this, Fsm);
             idle = new Idle(this, Fsm);
             Fsm.Initialize(idle);

            
        }

        void Update()
        {

            //Tick parameters
            Hunger += 1 * Time.deltaTime;
            Thirst++;
            
            //Handle Input
            Fsm.HandleStatesInput();
            
            //Update Logic
            Fsm.UpdateStatesLogic();

            if (Hungry())
            {
                Fsm.ChangeState(fs);
            }

            // if (agent.remainingDistance < 1.0f){
            //     agent.destination = Random.insideUnitCircle * 20;
            // }
        }
    
        private void FixedUpdate()
        {
            //Update physics
            Fsm.UpdateStatesPhysics();
        }
        
        
        
        
        //  OTHER METHODS
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
    }
}
