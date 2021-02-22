using System;
using System.Collections.Generic;
using AnimalsV2.States;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

//Author: Alexander LV, Johan A
// Heavily Inspired by: https://blog.playmedusa.com/a-finite-state-machine-in-c-for-unity3d/
// Used Unity Official Tutorial on the Animator


namespace AnimalsV2
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AnimalController))]
    public class Animal : MonoBehaviour
    {
    
        public NavMeshAgent agent;
    
        public FiniteStateMachine Fsm;
        public SearchForMate sm;
        public SearchForFood sf;
        public SearchForWater sw;
        public FleeingState fs;
        public Eating es;
        public Idle idle;

        // Controller of the animal
        private AnimalController animalController;


        
        public List<GameObject> heardTargets;
        public List<GameObject> visibleTargets;
        void Awake()
        {
            
            //Init internal Animal  model.
            animalController = GetComponent<AnimalController>();

            // Init the NavMesh agent
            agent = GetComponent<NavMeshAgent>();
            agent.autoBraking = false;

            //Create the FSM.
            Fsm = new FiniteStateMachine();
            AnimationController animationController = new AnimationController(this);
            
        }

        private void Start()
        {
            
             sf = new SearchForFood(this, Fsm);
             sw = new SearchForWater(this, Fsm);
             sm = new SearchForMate(this, Fsm);
             es = new Eating(this, Fsm);
             fs = new FleeingState(this, Fsm);
             idle = new Idle(this, Fsm);
             Fsm.Initialize(idle);
             
             

            
        }

        void Update()
        {

            //Get information from senses
            //animalModel.
            // heardTargets = animalController.heardTargets;
            // visibleTargets = animalController.visibleTargets;
            
            //Handle Input
            Fsm.HandleStatesInput();
            
            //Update Logic
            Fsm.UpdateStatesLogic();
            
            // if (agent.remainingDistance < 1.0f){
            //     agent.destination = Random.insideUnitCircle * 20;
            // }
        }
    
        private void FixedUpdate()
        {
            //Update physics
            Fsm.UpdateStatesPhysics();
        }
        
        
        
    }
}
