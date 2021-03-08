using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearingAbility : MonoBehaviour
{

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */

    [HideInInspector]
    public float radius;

    [SerializeField]
    private LayerMask targetMask;

    [HideInInspector]
    public AnimalController animalController;

    private TickEventPublisher tickEventPublisher;

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindHeardTargets()
    {
        // prevent adding duplicates
        //animalController.heardTargets.Clear(); // obsolete
        
        animalController.heardHostileTargets.Clear();
        animalController.heardFriendlyTargets.Clear();
        animalController.heardPreyTargets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            GameObject target = targetsInRadius[i].gameObject;
            
            
            // don't add self
            if (target != gameObject)
            {
                if (target.gameObject.CompareTag("Animal")) 
                    HandleAnimalTarget(target);
            }
        }
    }

    private void HandleAnimalTarget(GameObject target)
    {
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();

        //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
        if (targetAnimalController.animalModel.CanEat(animalController.animalModel))
        {
            animalController.heardHostileTargets.Add(target);
            animalController.actionPerceivedHostile?.Invoke(target);
        }
        //if the target is of same species: add to visibleFriendlyTargets
        else if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel))
        {
            animalController.heardFriendlyTargets.Add(target);
        }
        //if this animalModel can the targets animalModel: add to visibleFoodTargets
        else if (animalController.animalModel.CanEat(targetAnimalController.animalModel))
        {
            animalController.heardPreyTargets.Add(target);
        }
    }
    
    private void Start()
    {
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        animalController = GetComponent<AnimalController>();
        // set animals hearing distance
        radius = animalController.animalModel.traits.hearingRadius;
        
        // subscribe to Ticks
        tickEventPublisher.onSenseTickEvent += FindHeardTargets;
    }

    private void OnDestroy()
    {
        // unsubscribe from Ticks
        tickEventPublisher.onSenseTickEvent -= FindHeardTargets;
    }
}
