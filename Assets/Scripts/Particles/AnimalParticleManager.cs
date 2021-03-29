using System;
using System.Collections;
using System.Collections.Generic;
using AnimalsV2;
using AnimalsV2.States;
using AnimalsV2.States.AnimalsV2.States;
using UnityEngine;

public class AnimalParticleManager : MonoBehaviour
{
    //Make sure "Play on Awake is OFF."
    public ParticleSystem deathParticleSystem;
    public ParticleSystem smokeTrailParticleSystem;
    public ParticleSystem hitParticleSystem;
    public ParticleSystem matingParticleSystem;
    public ParticleSystem bornParticleSystem;
    

    private AnimalController animalController;
    private FiniteStateMachine fsm;

    private void Awake()
    {
        animalController = GetComponent<AnimalController>();
        
    }

    void Start()
    {
        fsm = animalController.fsm;
        InitializeParticleSystems();

        EventSubscribe();
    }

    private void InitializeParticleSystems()
    {
        if (deathParticleSystem != null)
        {
            deathParticleSystem = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
            deathParticleSystem.transform.parent = gameObject.transform;
        }

        if (smokeTrailParticleSystem != null)
        {
            smokeTrailParticleSystem = Instantiate(smokeTrailParticleSystem, transform.position, Quaternion.identity);
            smokeTrailParticleSystem.transform.parent = gameObject.transform;
        }
    }

    private void EventSubscribe()
    {
        animalController.fsm.OnStateEnter += ShowStateParticles;
        animalController.actionDeath += ShowDeathParticles;
    }
    private void EventUnsubscribe()
    {
        if (fsm != null)
        {
            animalController.fsm.OnStateEnter -= ShowStateParticles;
        }

        animalController.actionDeath -= ShowDeathParticles;
    }

    private void ShowStateParticles(State state)
    {
        //Reset particle systems.
        smokeTrailParticleSystem.Stop();
        
        if (state is MatingState)
        {
            
        }
        else if (state is FleeingState)
        {
            if (smokeTrailParticleSystem != null)
            {
          
                smokeTrailParticleSystem.Play();

            }
        }else if (state is Dead)
        {
            if (deathParticleSystem != null)
            {
                deathParticleSystem.Play();
            }
        }
    }

    private void ShowDeathParticles()
    {
        
    }

    private void OnDestroy()
    {
        EventUnsubscribe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
