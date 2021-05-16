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
    public ParticleSystem pregnancyParticleSystem;

    private DestroyParticle hitParticleSelfDestroyScript;
    
    private AnimalController animalController;
    private FiniteStateMachine fsm;

    private void Awake()
    {
        animalController = GetComponent<AnimalController>();
        
    }

    private void Start()
    {
        fsm = animalController.fsm;
        InitializeParticleSystems();

        EventSubscribe();
    }

    private void InitializeParticleSystems()
    {
        Quaternion upRotation = Quaternion.LookRotation(Vector3.up, Vector3.forward);
        
        if (deathParticleSystem)
        {
            deathParticleSystem = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
            deathParticleSystem.transform.parent = gameObject.transform;
        }

        if (smokeTrailParticleSystem)
        {
            smokeTrailParticleSystem = Instantiate(smokeTrailParticleSystem, transform.position, Quaternion.identity);
            smokeTrailParticleSystem.transform.parent = gameObject.transform;
        }

        if (hitParticleSystem)
        {
            hitParticleSystem = Instantiate(hitParticleSystem, transform.position, Quaternion.identity);
            hitParticleSystem.transform.parent = gameObject.transform;
            hitParticleSelfDestroyScript = hitParticleSystem.GetComponent<DestroyParticle>();
        }

        if (matingParticleSystem)
        {
            matingParticleSystem = Instantiate(matingParticleSystem, transform.position, upRotation);
            matingParticleSystem.transform.parent = gameObject.transform;
        }

        if (pregnancyParticleSystem)
        {
            pregnancyParticleSystem = Instantiate(pregnancyParticleSystem, transform.position, upRotation);
            pregnancyParticleSystem.transform.parent = gameObject.transform;
        }

        if (bornParticleSystem)
        {
            bornParticleSystem = Instantiate(bornParticleSystem, transform.position, upRotation);
            bornParticleSystem.transform.parent = gameObject.transform;
        }
    }

    private void EventSubscribe()
    {
        animalController.fsm.OnStateEnter += ShowStateParticles;
        animalController.ActionPregnant += ShowPregnancyParticles;
        animalController.deadState.onDeath += StopOnDeath;
        animalController.animalModel.actionKilled += ShowHitParticles;
    }
    private void EventUnsubscribe()
    {
        if (fsm != null)
        {
            animalController.fsm.OnStateEnter -= ShowStateParticles;
        }

        animalController.ActionPregnant -= ShowPregnancyParticles;
        
        animalController.deadState.onDeath -= StopOnDeath;
        animalController.animalModel.actionKilled -= ShowHitParticles;
    }

    private void ShowStateParticles(State state)
    {
        //Reset particle systems.
        smokeTrailParticleSystem.Stop();
        matingParticleSystem.Stop();

        switch (state)
        {
            case MatingState _:
            {
                
                if (matingParticleSystem)
                {
                    matingParticleSystem.Play();
                }

                break;
            }
            case FleeingState _:
            {
                if (smokeTrailParticleSystem)
                {
          
                    smokeTrailParticleSystem.Play();

                }

                break;
            }
            case Dead _:
            {
                if (deathParticleSystem)
                {
                    deathParticleSystem.Play();
                }

                break;
            }
        }
    }

    private void ShowPregnancyParticles(bool isPregnant)
    {
        if (pregnancyParticleSystem)
        {
            if (isPregnant)
            {
                
                pregnancyParticleSystem.Play();
            }
            else
            {
                pregnancyParticleSystem.Stop();
            }
        }
    }

    private void ShowHitParticles()
    {
        if (hitParticleSystem && hitParticleSelfDestroyScript != null)
        {
            hitParticleSelfDestroyScript.SelfDestructAfterPlayAndDetach();
        }
        StopOnDeath(null, false);
    }

    private void StopOnDeath(AnimalController am, bool gotEaten)
    {
        matingParticleSystem.Stop();
        bornParticleSystem.Stop();
        pregnancyParticleSystem.Stop();
        smokeTrailParticleSystem.Stop();
    }

    private void OnDestroy()
    {
        StopOnDeath(null, false);
        EventUnsubscribe();
    }
}
