using System;
using System.Collections;
using System.Collections.Generic;
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
        animalController.visibleTargets.Clear();
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
                float distToTarget = Vector3.Distance(transform.position, target.transform.position);

                // if target is not obscured
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    animalController.visibleTargets.Add(target);
                    // for custom editor FoVEditor
                    targets.Add(target);
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
        
        angle = animalController.animal.traits.viewAngle;
        radius = animalController.animal.traits.viewRadius;

        FindObjectOfType<global::TickEventPublisher>().onSenseTickEvent += FindVisibleTargets;
    }

    private void FixedUpdate()
    {
        angle = animalController.animal.traits.viewAngle;
        radius = animalController.animal.traits.viewRadius;
    }
}
