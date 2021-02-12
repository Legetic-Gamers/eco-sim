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

    public List<Transform> targets = new List<Transform>();

    private AnimalTraitModel animalModel;

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindVisibleTargets()
    {
        // prevent adding duplicates
        targets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        // loop through targets within the entire circle to determine if they are in the view cone --> add to Targets list
        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Transform target = targetsInRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                // if target is not obscured
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask))
                {
                    targets.Add(target);
                    // handle target added somehow: flee from, move closer, etc
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

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    
    private void Start() 
    {
        animalModel = GetComponent<AnimalTraitModel>();
        
        angle = animalModel.viewAngle;
        radius = animalModel.viewRadius;

        // runs FindTargets every delay
        StartCoroutine("FindTargetsWithDelay", .5f);
    }
}
