using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using ViewController;
using Debug = UnityEngine.Debug;

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
    
    [HideInInspector]
    public AnimalController animalController;

    private TickEventPublisher tickEventPublisher;
    
    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindVisibleTargets()
    {
        // prevent adding duplicates
        animalController.visibleHostileTargets.Clear();
        animalController.visibleFriendlyTargets.Clear();
        animalController.visibleFoodTargets.Clear();
        animalController.visibleWaterTargets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        // loop through targets within the entire circle to determine if they are in the view cone --> add to Targets list
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            GameObject target = targetsInRadius[i].gameObject;

            // don't add self
            if (target == gameObject) return;

            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                //Debug.Log(target.name);
                float distToTarget = Vector3.Distance(transform.position, target.transform.position);

                // if target is not obscured
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    if(target.gameObject.CompareTag("Plant")) 
                        HandlePlantTarget(target);
                    else if (target.gameObject.CompareTag("Animal")) 
                        HandleAnimalTarget(target);
                    else if (target.gameObject.CompareTag("Water"))
                        HandleWaterTarget(target);
                }
            }
        }
    }

    private void HandleAnimalTarget(GameObject target)
    {
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();

        //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
        if (targetAnimalController.animalModel.CanEat(animalController.animalModel))
        {
            animalController.visibleHostileTargets.Add(target);
            animalController.actionPerceivedHostile?.Invoke(target);
        }  
        //if this animalModel can the targets animalModel: add to visibleFoodTargets
        else if (animalController.animalModel.CanEat(targetAnimalController.animalModel))
        {
            animalController.visibleFoodTargets.Add(target);
        }
        //if the target is of same species: add to visibleFriendlyTargets
        else if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel))
        {
            animalController.visibleFriendlyTargets.Add(target);
        }
    }
    
    private void HandleWaterTarget(GameObject target)
    {
        animalController.visibleWaterTargets.Add(target);
    }

    private void HandlePlantTarget(GameObject target)
    {
        PlantController targetPlantController = target.GetComponent<PlantController>();
        if (animalController.animalModel.CanEat(targetPlantController.plantModel))
        {
            animalController.visibleFoodTargets.Add(target);
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
        tickEventPublisher = FindObjectOfType<global::TickEventPublisher>();
        
        animalController = GetComponent<AnimalController>();
        
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;

        tickEventPublisher.onSenseTickEvent += FindVisibleTargets;
    }

    private void OnDestroy()
    {
        tickEventPublisher.onSenseTickEvent -= FindVisibleTargets;
    }

    private void FixedUpdate()
    {
        angle = animalController.animalModel.traits.viewAngle;
        radius = animalController.animalModel.traits.viewRadius;
    }
}
