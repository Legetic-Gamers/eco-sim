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
            
            
            // don't add self
            if (target != gameObject)
            {
                // for custom editor HAEditor
                targets.Add(target);

                HandleAnimalTarget(target);
            }
        }
    }

    private void HandleAnimalTarget(GameObject target)
    {
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();

        if (targetAnimalController != null)
        {
            //if this animalModel can the targets animalModel: add to visibleFoodTargets
            if (animalController.animalModel.CanEat(targetAnimalController.animalModel))
            {
                animalController.heardPreyTargets.Add(target);
                return;
            }
            //if the target is of same species: add to visibleFriendlyTargets
            if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel))
            {
                animalController.heardFriendlyTargets.Add(target);
                return;
            }
            //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
            if (targetAnimalController.animalModel.CanEat(animalController.animalModel))
            {
                animalController.heardHostileTargets.Add(target);
                animalController.animalModel.actionPerceivedHostile?.Invoke(target);
                return;
            }
        }
        
    }
    
    /*
    private void HandleTarget(GameObject target)
    {
        // can't hear non-animals, i.e plants/water
        if (!target.gameObject.CompareTag("Animal")) return;
        
        AnimalController targetAC = target.GetComponent<AnimalController>();
        
        // hear same species -> mate
        if (animalController.IsSameSpecies(targetAC))
        {
            animalController.heardFriendlyTargets.Add(target);
        }
        // hear predator
        else if (targetAC.IsPredator)
        {
            animalController.heardHostileTargets.Add(target);
            animalController.animalModel.actionPerceivedHostile?.Invoke(target);
        }
        // hear prey, self is predator
        else if (animalController.IsPredator && targetAC.IsPrey)
        {
            animalController.heardPreyTargets.Add(target);
        }
    }
    */
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
