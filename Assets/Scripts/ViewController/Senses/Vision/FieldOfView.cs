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

    // custom editor needs this (otherwise will get an error), remove once custom editor is obsolete, or when stress testing
    public List<GameObject> targets = new List<GameObject>();

    [HideInInspector]
    public AnimalController animalController;

    private TickEventPublisher tickEventPublisher;
    
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
                    // for custom editor FoVEditor
                    targets.Add(target);

                    
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

        if (targetAnimalController != null)
        {
            //if this animalModel can the targets animalModel: add to visibleFoodTargets
            if (animalController.animalModel.CanEat(targetAnimalController.animalModel))
            {
                animalController.visibleFoodTargets.Add(target);
                return;
            }
            //if the target is of same species: add to visibleFriendlyTargets
            if (animalController.animalModel.IsSameSpecies(targetAnimalController.animalModel))
            {
                animalController.visibleFriendlyTargets.Add(target);
                return;
            }
            //if the targets animalModel can eat this animalModel: add to visibleHostileTargets
            if (targetAnimalController.animalModel.CanEat(animalController.animalModel))
            {
                animalController.visibleHostileTargets.Add(target);
                animalController.animalModel.actionPerceivedHostile?.Invoke(target);
                return;
            }    
        }
    }
    
    private void HandleWaterTarget(GameObject target)
    {
        animalController.visibleWaterTargets.Add(target);
    }

    private void HandlePlantTarget(GameObject target)
    {
        PlantController targetPlantController = target.GetComponent<PlantController>();
        if (targetPlantController != null && animalController.animalModel.CanEat(targetPlantController.plantModel))
        {
            animalController.visibleFoodTargets.Add(target);
        }
    }
    /* I have created a replacement // Alexander Huang
    private void HandleConsumableTarget(GameObject target)
    {
        // see water
        if (target.gameObject.CompareTag("Water"))
        {
            animalController.visibleWaterTargets.Add(target);
        }
        // see plant
        else if (animalController.IsPrey && target.gameObject.CompareTag("Food"))
        {
            animalController.visibleFoodTargets.Add(target);
        }
    }
    */
    
    /* I have created a replacement // Alexander Huang
    private void HandleAnimalTarget(GameObject target)
    {
        AnimalController targetAnimalController = target.GetComponent<AnimalController>();
        
        // see same species -> mate
        if (animalController.IsSameSpecies(targetAnimalController))
        {
            animalController.visibleFriendlyTargets.Add(target);
            return;
        }
        // see predator
        if (targetAnimalController.IsPredator)
        {
            animalController.visibleHostileTargets.Add(target);
            animalController.animalModel.actionPerceivedHostile?.Invoke(target);
            return;
        }
        // see prey
        if (animalController.IsPredator && targetAnimalController.IsPrey)
        {
            animalController.visibleFoodTargets.Add(target);
        }
    }
*/
    

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
