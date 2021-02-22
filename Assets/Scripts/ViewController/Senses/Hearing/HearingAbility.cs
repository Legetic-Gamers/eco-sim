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

    // custom editor needs this (will get an error if using animalController's lists instead),
    // remove once custom editor is obsolete, or when stress testing
    public List<GameObject> targets = new List<GameObject>();

    public AnimalController animalController;
    public bool isPrey;

    private TickEventPublisher tickEventPublisher;

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindHeardTargets()
    {
        // prevent adding duplicates
        //animalController.heardTargets.Clear(); // obsolete
        
        animalController.heardHostileTargets.Clear();
        animalController.heardFriendlyTargets.Clear();
        animalController.heardPreyTargets.Clear();
        // for custom editor HAEditor
        targets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            GameObject target = targetsInRadius[i].gameObject;
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();
            
            // don't add self
            if (target != gameObject && targetAnimalController != null)
            {
                
                //animalController.heardTargets.Add(target);
                // for custom editor HAEditor
                targets.Add(target);

                if (isPrey && targetAnimalController.animalModel.traits.IsPredator)
                {
                    animalController.heardHostileTargets.Add(target);
                    animalController.animalModel.actionPerceivedHostile?.Invoke(target);
                }
                else if (!isPrey && targetAnimalController.animalModel.traits.IsPrey)
                {
                    animalController.heardPreyTargets.Add(target);
                }
                else if (animalController.IsSameSpecies(targetAnimalController))
                {
                    animalController.heardFriendlyTargets.Add(target);
                }
            }
        }
    }
    
    private void Start()
    {
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        animalController = GetComponent<AnimalController>();
        if (animalController.animalModel.traits.behaviorType == Traits.BehaviorType.Herbivore) isPrey = true;
        
        radius = animalController.animalModel.traits.hearingRadius;
        
        tickEventPublisher.onSenseTickEvent += FindHeardTargets;
    }

    private void OnDestroy()
    {
        tickEventPublisher.onSenseTickEvent -= FindHeardTargets;
    }

    private void FixedUpdate()
    {
        radius = animalController.animalModel.traits.hearingRadius;
    }
}
