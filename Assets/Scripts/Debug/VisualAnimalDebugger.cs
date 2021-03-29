using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimalsV2;
using AnimalsV2.States;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using ViewController;

[RequireComponent(typeof(NavMeshAgent))]
//[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AnimalController))]
public class VisualAnimalDebugger: MonoBehaviour
{
    [SerializeField] private bool showNavMeshAgentPath;

    [SerializeField] private bool allowNavigateWithClick;

    [SerializeField] private bool showVisibleTargets;

    [SerializeField] private bool showFleeingVector;
    
    [SerializeField] private bool showPerceptionRange;


    private AnimalController animalController;
    private NavMeshAgent navmeshAgent;
    private LineRenderer lineRenderer;
    private Camera camera;

    // declaring an action using built-in EventHandler
    public event Action debugHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        animalController = GetComponent<AnimalController>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        if (TryGetComponent(out LineRenderer lr))
        {
            lineRenderer = lr;
            lineRenderer.widthMultiplier = 0.05f;
        }
        
        camera = Camera.main;

        if (showNavMeshAgentPath && lineRenderer != null) debugHandler += ShowNavMeshAgentPath;
        if (allowNavigateWithClick) debugHandler += NavigateWithClick;
        if (showVisibleTargets) debugHandler += ShowVisibleTargetLines;
        if (showFleeingVector) debugHandler += ShowFleeingVector;
        // if (showPerceptionRange) debugHandler += ShowPerceptionRange;
    }

    

    private void OnDestroy()
    {
        if (showNavMeshAgentPath && lineRenderer != null) debugHandler -= ShowNavMeshAgentPath;
        if (allowNavigateWithClick) debugHandler -= NavigateWithClick;
        if (showVisibleTargets) debugHandler -= ShowVisibleTargetLines;
        if (showFleeingVector) debugHandler -= ShowFleeingVector;
        // if (showPerceptionRange) debugHandler -= ShowPerceptionRange;
    }

    // Update is called once per frame
    void Update()
    {
        debugHandler?.Invoke();
    }

    private void ShowFleeingVector()
    {
        var pos = animalController.transform.position;
        List<GameObject> allHostileTargets = animalController.heardHostileTargets.Concat(animalController.visibleHostileTargets).ToList();
        Vector3 pointToAnimalVector = NavigationUtilities.GetNearObjectsAveragePosition(allHostileTargets, pos);
        Vector3 pointToRunTo = NavigationUtilities.RunFromPoint(animalController.transform,pointToAnimalVector);

        if (animalController.fsm.currentState is FleeingState)
        {
            Vector3 pathEnd = navmeshAgent.path.corners[navmeshAgent.path.corners.Length -1];

            Debug.DrawLine(pos, pathEnd, Color.yellow);
        }
    }
    
    private void ShowPerceptionRange()
    {
        
    }

    

    void OnDrawGizmosSelected()
    {
        if (showPerceptionRange && animalController != null)
        {
            //Hearing
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, animalController.animalModel.traits.hearingRadius);   
        
            //Sight
            //https://answers.unity.com/questions/21176/gizmo-question-how-do-i-create-a-field-of-view-usi.html
            float totalFOV = animalController.animalModel.traits.viewAngle;
            float rayRange = animalController.animalModel.traits.viewRadius;
            float halfFOV = totalFOV / 2.0f;
            Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
            Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
            Vector3 leftRayDirection = leftRayRotation * transform.forward;
            Vector3 rightRayDirection = rightRayRotation * transform.forward;
            Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
            Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
        

            Debug.Log("Perception");
        }
    }

    private void ShowNavMeshAgentPath()
    {
        if (navmeshAgent.hasPath)
        {
            lineRenderer.positionCount = navmeshAgent.path.corners.Length;
            lineRenderer.SetPositions(navmeshAgent.path.corners);
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
    
    

    private void NavigateWithClick()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                navmeshAgent.SetDestination(hit.point);
            }
        }
    }

    private void ShowVisibleTargetLines()
    {
        //Show line to food
        foreach (GameObject food in animalController.visibleFoodTargets.Concat(animalController.heardPreyTargets))
        {
            if (food != null)
            {
                Debug.DrawLine(gameObject.transform.position, food.transform.position, Color.green);
            }
        }

        //Show line to friendlies, potential mates have a magenta line
        foreach (GameObject friendly in animalController.visibleFriendlyTargets.Concat(animalController.heardFriendlyTargets))
        {
            
            if (friendly != null && friendly.TryGetComponent(out AnimalController otherAnimalController))
            {
                if (otherAnimalController.animalModel.WantingOffspring && animalController.animalModel.WantingOffspring)
                {
                    Debug.DrawLine(gameObject.transform.position, friendly.transform.position, Color.magenta);
                }
                else
                {
                    Debug.DrawLine(gameObject.transform.position, friendly.transform.position, Color.white);
                }
            }
        }

        //Show line to hostiles
        foreach (GameObject hostile in animalController.heardHostileTargets.Concat(animalController.visibleHostileTargets))
        {
            if(hostile != null)
                Debug.DrawLine(gameObject.transform.position, hostile.transform.position, Color.red);
        }

        //Show line to water sources
        foreach (GameObject water in animalController.visibleWaterTargets)
        {
            if(water != null)
                Debug.DrawLine(gameObject.transform.position, water.transform.position, Color.blue);
        }
        
    }
    
    
}
