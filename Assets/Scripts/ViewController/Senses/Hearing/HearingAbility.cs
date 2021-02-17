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

    // custom editor needs this (otherwise will get an error), remove once custom editor is obsolete, or when stress testing
    public List<GameObject> targets = new List<GameObject>();

    public AnimalController animalController;
   
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

            // don't add self
            if (target != gameObject)
            {
                animalController.heardTargets.Add(target);
                // for custom editor HAEditor
                targets.Add(target);
            }
        }
    }
    
    private void Start()
    {
        animalController = GetComponent<AnimalController>();
        radius = animalController.animal.traits.hearingRadius;
        
        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += FindHeardTargets;
    }
    
    private void FixedUpdate()
    {
        radius = animalController.animal.traits.hearingRadius;
    }
}
