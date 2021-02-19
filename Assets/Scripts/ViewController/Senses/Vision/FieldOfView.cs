using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */

    [HideInInspector]
    [Range(0, 360)]
    public float angle;
    [HideInInspector]
    public float radius;

    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstacleMask;

    // custom editor needs this (otherwise will get an error), remove once custom editor is obsolete, or when stress testing
    public List<GameObject> targets = new List<GameObject>();

    [HideInInspector]
    public AnimalController animalController;
    private bool isPrey;
    
    public delegate void ScoutedTargetDelegate();

    public event ScoutedTargetDelegate onSeenHostileEvent;
    public event ScoutedTargetDelegate onSeenFriendlyEvent;
    public event ScoutedTargetDelegate onSeenFoodEvent; // either prey or plants

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindVisibleTargets()
    {
        // prevent adding duplicates
        animalController.visibleTargets.Clear();
        // for custom editor FoVEditor
        targets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        // loop through targets within the entire circle to determine if they are in the view cone --> add to Targets list
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            GameObject target = targetsInRadius[i].gameObject;
            AnimalController targetAnimalController = target.GetComponent<AnimalController>();
            
            // don't add self
            if (target == gameObject) return;
            
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.transform.position);

                // if target is not obscured
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    animalController.visibleTargets.Add(target);
                    // for custom editor FoVEditor
                    targets.Add(target);

                    switch (isPrey)
                    {
                        case true: 
                            if (targetAnimalController.animalModel.traits.IsCarnivore) 
                                onSeenHostileEvent?.Invoke();
                            /*
                             * not herbivore and not carnivore/omnivore (above) -> must be a plant.
                             * 
                             * should probably have two targetMask, one for predators to see only prey and other predators,
                             * and one for herbivores to see herbivores, predators, and plants
                             */
                            else if (!targetAnimalController.animalModel.traits.IsHerbivore) 
                                onSeenFoodEvent?.Invoke();
                            break;
                        case false: 
                            if (targetAnimalController.animalModel.traits.IsHerbivore)
                                onSeenFoodEvent?.Invoke();
                            break;
                    }
                    
                    if (animalController.IsSameSpecies(targetAnimalController))
                        onSeenFriendlyEvent?.Invoke();
                    
                }
            }
        }
    }

    // get angle direction
    public Vector3 DirectionOfAngle(float angleInDegrees)
    {
        // make it global
        angleInDegrees += transform.eulerAngles.y;
        
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    /* /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\ */

    private void Start() 
    {
        animalController = GetComponent<AnimalController>();
        
        if (animalController.animalModel.traits.behaviorType == Traits.BehaviorType.Herbivore) isPrey = true;
        
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;

        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += FindVisibleTargets;
    }

    private void FixedUpdate()
    {
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;
    }
}
