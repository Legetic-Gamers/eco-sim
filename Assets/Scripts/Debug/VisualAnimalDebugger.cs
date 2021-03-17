using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AnimalController))]
public class VisualAnimalDebugger: MonoBehaviour
{
    [SerializeField] private bool showNavMeshAgentPath;

    [SerializeField] private bool allowNavigateWithClick;

    [SerializeField] private bool showVisibleTargets;


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
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.widthMultiplier = 0.05f;
        camera = Camera.main;

        if (showNavMeshAgentPath) debugHandler += ShowNavMeshAgentPath;
        if (allowNavigateWithClick) debugHandler += NavigateWithClick;
        if (showVisibleTargets) debugHandler += ShowVisibleTargetLines;
    }

    // Update is called once per frame
    void Update()
    {
        debugHandler.Invoke();
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
        foreach (GameObject target in animalController.visibleFoodTargets.Concat(animalController.visibleWaterTargets.Concat(animalController.visibleHostileTargets.Concat(animalController.visibleFriendlyTargets))).ToList())
        {
            if (target)
            {
                Debug.DrawLine(gameObject.transform.position, target.transform.position, Color.green);
            }
        }
    }
    
    
}
