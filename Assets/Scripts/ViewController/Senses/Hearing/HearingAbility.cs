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
    
    public delegate void ScoutedTargetDelegate();

    public event ScoutedTargetDelegate onHeardHostileEvent;
    public event ScoutedTargetDelegate onHeardFriendlyEvent;
    public event ScoutedTargetDelegate onHeardFoodEvent;
   
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindHeardTargets()
    {
        // prevent adding duplicates
        animalController.heardTargets.Clear();
        // for custom editor HAEditor
        targets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            GameObject target = targetsInRadius[i].gameObject;
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();

            // don't add self
            if (target != gameObject)
            {
                animalController.heardTargets.Add(target);
                // for custom editor HAEditor
                targets.Add(target);
                
                if (isPrey && targetAnimalController.animal.traits.IsCarnivore) 
                    animalController.animal.actionPerceivedHostile?.Invoke(target);
                else if (!isPrey && targetAnimalController.animal.traits.IsHerbivore)
                    animalController.animal.actionPerceivedFood?.Invoke(target);
                else if (animalController.IsSameSpecies(targetAnimalController))
                    animalController.animal.actionPerceivedFriendly?.Invoke(target);
            }
        }
    }
    
    private void Start()
    {
        animalController = GetComponent<AnimalController>();
        
        if (animalController.animalModel.traits.behaviorType == Traits.BehaviorType.Herbivore) isPrey = true;
        
        // Debug.Log("YOO");
        // Debug.Log(animalController.animalModel);
        radius = animalController.animalModel.traits.hearingRadius;
        
        
        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += FindHeardTargets;
    }
    
    private void FixedUpdate()
    {
        radius = animalController.animalModel.traits.hearingRadius;
    }
}
