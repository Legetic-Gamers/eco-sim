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
    
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindVisibleTargets()
    {
        // prevent adding duplicates
        //animalController.visibleTargets.Clear(); // obsolete
        
        animalController.visibleHostileTargets.Clear();
        animalController.visibleFriendlyTargets.Clear();
        animalController.visiblePreyTargets.Clear();
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
                    // obsolete with the invokes below
                    //animalController.visibleTargets.Add(target);
                    // for custom editor FoVEditor
                    targets.Add(target);

                    switch (animalController.animalModel.traits.IsPrey)
                    {
                        case true:
                            if (targetAnimalController.animalModel.traits.IsPredator)
                            {
                                animalController.visibleHostileTargets.Add(target);
                                animalController.animalModel.actionPerceivedHostile?.Invoke(target);
                            }
                            /*
                             * not herbivore and not carnivore/omnivore (above) -> must be a plant.
                             * 
                             * problem however is that plants don't have a behaviorType, so this will
                             * lead to a NullReferenceException if we try to do the following if() statement
                             */
                            else if (!targetAnimalController.animalModel.traits.IsPrey)
                            {
                                // do something
                            }
                            break;
                        case false:
                            if (targetAnimalController.animalModel.traits.IsPrey)
                            {
                                animalController.visiblePreyTargets.Add(target);
                            }
                            break;
                    }

                    if (animalController.IsSameSpecies(targetAnimalController))
                    {
                        animalController.visibleFriendlyTargets.Add(target);
                    }
                    
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
        
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;

        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += FindVisibleTargets;
    }

    private void OnDestroy()
    {
        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent -= FindVisibleTargets;
    }

    private void FixedUpdate()
    {
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;
    }
}
