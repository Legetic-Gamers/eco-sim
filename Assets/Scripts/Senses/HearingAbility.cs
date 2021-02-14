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
    [SerializeField]
    private LayerMask obstacleMask;

    public List<Transform> targets = new List<Transform>();

    private AnimalModel animalModel;

    /* \/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/ */

    private void FindHeardTargets()
    {
        // prevent adding duplicates
        targets.Clear();

        // add targets in list when they enter the sphere
        Collider[] targetsInRadius = Physics.OverlapSphere(transform.position, radius, targetMask);

        for (int i = 0; i < targetsInRadius.Length; i++)
        {
            Transform target = targetsInRadius[i].transform;

            targets.Add(target);

            // handle target added using delegate or by setting a state: turn towards, look at, flee from, etc
        }
    }

    // runs FindTargets every delay
    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindHeardTargets();
        }
    }
    
    private void Start()
    {
        animalModel = GetComponent<AnimalModel>();
        radius = animalModel.hearingRadius;

        StartCoroutine("FindTargetsWithDelay", .5f);
    }
}
