using System;
using System.Collections.Generic;
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
    [RequireComponent(typeof(NavMeshAgent))]

    public class Animal : MonoBehaviour
    {
    
        public NavMeshAgent agent;
    
        public StateMachine Fsm;
        public SearchForMate sm;
        public SearchForFood sf;
        public SearchForWater sw;
        public FleeingState fs;
        public Idle idle;

        // Internal representation. Traits, parameters and senses of the animal
        private AnimalModel animalModel;


        [HideInInspector] 
        public List<GameObject> heardTargets;
        [HideInInspector] 
        public List<GameObject> visibleTargets;
        void Awake()
        {
            
            //Init internal Animal  model.
            animalModel = GetComponent<AnimalModel>();

            // Init the NavMesh agent
            agent = GetComponent<NavMeshAgent>();
            agent.autoBraking = false;

            //Create the FSM.
            Fsm = new StateMachine();
            AnimationController animationController = new AnimationController(this);
            
        }

        private void Start()
        {
            
            // sf = new SearchForFood(this, Fsm);
            // sw = new SearchForWater(this, Fsm);
            // sm = new SearchForMate(this, Fsm);
             fs = new FleeingState(this, Fsm);
             idle = new Idle(this, Fsm);
             Fsm.Initialize(fs);

            
        }

        void Update()
        {

            //Get information from senses
            //animalModel.
            heardTargets = animalModel.heardTargets;
            visibleTargets = animalModel.visibleTargets;
            
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
